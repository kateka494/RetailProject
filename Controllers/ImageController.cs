using Microsoft.AspNetCore.Mvc;
using RetailProject.Models;
using RetailProject.Services;

namespace RetailProject.Controllers
{
    public class ImageController : Controller
    {
        private readonly BlobStorageService _blobService;
        private readonly QueueStorageService _queueService;

        public ImageController(
            BlobStorageService blobService,
            QueueStorageService queueService)
        {
            _blobService = blobService;
            _queueService = queueService;
        }

        // Shows all images stored in Blob Storage
        public async Task<IActionResult> Index()
        {
            var allImages =
                await _blobService.ListProductImagesAsync(prefix: "");

            return View(allImages);
        }

        // Uploads an image to Blob Storage and sends a queue message
        [HttpPost]
        public async Task<IActionResult> Upload(
            IFormFile file,
            string productId)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                productId = "general";
            }

            await _blobService.UploadImageAsync(file, productId);

            // Notify the image processing queue after the upload
            await _queueService.SendImageAsync(new ImageQueueMessage
            {
                ImageName = file.FileName,
                ProductId = productId,
                Action = "UploadImage"
            });

            return RedirectToAction("Index");
        }

        // Downloads an image from Blob Storage
        public async Task<IActionResult> Download(string blobName)
        {
            try
            {
                var stream =
                    await _blobService.DownloadImageAsync(blobName);

                var fileName = Path.GetFileName(blobName);

                return File(
                    stream,
                    "application/octet-stream",
                    fileName);
            }
            catch
            {
                return NotFound();
            }
        }

        // Deletes an image from Blob Storage
        [HttpPost]
        public async Task<IActionResult> Delete(string blobName)
        {
            await _blobService.DeleteImageAsync(blobName);

            return RedirectToAction("Index");
        }
    }
}