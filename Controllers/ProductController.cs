using Microsoft.AspNetCore.Mvc;
using RetailProject.Models;
using RetailProject.Services;

namespace RetailProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableStorageService _tableService;
        private readonly BlobStorageService _blobService;
        private readonly QueueStorageService _queueService;
        private readonly FileShareService _fileService;

        public ProductController(
            TableStorageService tableService,
            BlobStorageService blobService,
            QueueStorageService queueService,
            FileShareService fileService)
        {
            _tableService = tableService;
            _blobService = blobService;
            _queueService = queueService;
            _fileService = fileService;
        }

        // GET: List all products
        public async Task<IActionResult> Index()
        {
            var products = await _tableService.GetProductsAsync();
            return View(products);
        }

        // GET: Show Create Form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handle Create Form
        [HttpPost]
        public async Task<IActionResult> Create(string productName, string productCategory, decimal productPrice, string productDescription, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    RowKey = Guid.NewGuid().ToString(),
                    ProductName = productName,
                    ProductCategory = productCategory,
                    ProductPrice = productPrice,
                    ProductDescription = productDescription,
                    DateAdded = DateTime.UtcNow,
                    IsActive = true
                };

                if (imageFile != null && imageFile.Length > 0)
                {
                    var blobName = await _blobService.UploadImageAsync(imageFile, product.ProductId);
                    var blobUrl = _blobService.GetImageUrl(blobName);
                    product.ImageUrls = blobUrl;
                    product.ImageBlobNames = blobName;
                }

                await _tableService.SaveProductAsync(product);
                return RedirectToAction("Index");
            }

            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index"); 
            }

            var product = await _tableService.GetProductAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // EDIT (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(string productId, string productName, string productCategory, decimal productPrice, string productDescription)
        {
            var product = await _tableService.GetProductAsync(productId);
            if (product == null) return NotFound();

            product.ProductName = productName;
            product.ProductCategory = productCategory;
            product.ProductPrice = productPrice;
            product.ProductDescription = productDescription;

            await _tableService.SaveProductAsync(product);
            return RedirectToAction("Index");
        }

        // DELETE (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(string productId)
        {
            if (string.IsNullOrEmpty(productId)) return RedirectToAction("Index");

            var product = await _tableService.GetProductAsync(productId);
            if (product != null && !string.IsNullOrEmpty(product.ImageBlobNames))
            {
                await _blobService.DeleteImageAsync(product.ImageBlobNames);
            }

            await _tableService.DeleteProductAsync(productId);
            return RedirectToAction("Index");
        }

        // POST: "Place Order" Button
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string productId, decimal price)
        {
            string orderId = Guid.NewGuid().ToString();

            var orderMessage = new OrderQueueMessage
            {
                OrderId = orderId,
                CustomerId = "Test-Customer-001",
                Action = "ProcessPaymentAndShip",
                Amount = price,
                Timestamp = DateTime.UtcNow
            };
            await _queueService.SendOrderAsync(orderMessage);

            var inventoryMessage = new InventoryQueueMessage
            {
                ProductId = productId,
                Action = "DeductStock",
                Quantity = 1,
                Timestamp = DateTime.UtcNow
            };
            await _queueService.SendInventoryAsync(inventoryMessage);

            await _fileService.WriteLogAsync($"ORDER PLACED: ID {orderId} for Product {productId} at R{price}");

            TempData["Success"] = $"Order {orderId} placed! Processing started.";
            return RedirectToAction("Index");
        }
    }
}