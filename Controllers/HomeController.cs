using Microsoft.AspNetCore.Mvc;
using RetailProject.Models;
using RetailProject.Services;

namespace RetailProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly TableStorageService _tableStorage;
        private readonly BlobStorageService _blobStorage;

        public HomeController(
            TableStorageService tableStorage,
            BlobStorageService blobStorage)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            // Test the Azure Tables connection
            var customers = await _tableStorage.GetCustomersAsync();
            var products = await _tableStorage.GetProductsAsync();

            // Get product images from Blob Storage
            var blobImages = await _blobStorage.ListProductImagesAsync("");
            var productImages = new List<string>();

            foreach (var image in blobImages)
            {
                productImages.Add(image.Url);
            }

            ViewBag.CustomerCount = customers.Count;
            ViewBag.ProductCount = products.Count;
            ViewBag.TotalCustomers = customers.Count;
            ViewBag.TotalProducts = products.Count;
            ViewBag.ProductImages = productImages;

            // Get recent customers for display
            var recentCustomers = customers
                 .OrderByDescending(c => c.DateRegistered)
                .Take(3)
                .ToList();

            // Get featured products (with images if available)
            var featuredProducts = products
                .OrderByDescending(product => product.DateAdded)
                .Take(3)
                .ToList();

            // Load image URLs for featured products
            foreach (var product in featuredProducts)
            {
                if (!string.IsNullOrWhiteSpace(product.ImageBlobNames))
                {
                    var blobNames = product.ImageBlobNames
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    if (blobNames.Any())
                    {
                        var urls = _blobStorage.GetImageUrls(blobNames);
                        product.ImageUrls = string.Join(",", urls);
                    }
                }
            }

            ViewBag.RecentCustomers = recentCustomers;
            ViewBag.FeaturedProducts = featuredProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}