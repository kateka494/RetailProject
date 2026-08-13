using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using RetailProject.Models;
using RetailProject.Options;

namespace RetailProject.Services
{
    // Handles product images stored in Azure Blob Storage
    public class BlobStorageService
    {
        private readonly AzureStorageOptions _settings;
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<BlobStorageService> _logger;
        private readonly QueueStorageService _queueService; 

        public BlobStorageService(
            IOptions<AzureStorageOptions> settings,
            ILogger<BlobStorageService> logger,
            QueueStorageService queueService) 
        {
            _settings = settings.Value;
            _logger = logger;
            _queueService = queueService; 

            var blobServiceClient = new BlobServiceClient(
                _settings.ConnectionString);

            _containerClient = blobServiceClient.GetBlobContainerClient(
                _settings.BlobContainer);
        }

        public async Task InitializeAsync()
        {
            await _containerClient.CreateIfNotExistsAsync();
        }

        // Uploads an image and sends a Queue message
        public async Task<string> UploadImageAsync(IFormFile imageFile, string productId)
        {
            try
            {
                var extension = Path.GetExtension(imageFile.FileName);
                var blobName = $"{productId}/{Guid.NewGuid()}{extension}";

                var blobClient = _containerClient.GetBlobClient(blobName);

                var httpHeaders = new BlobHttpHeaders
                {
                    ContentType = imageFile.ContentType
                };

                using var stream = imageFile.OpenReadStream();
                await blobClient.UploadAsync(stream, httpHeaders);

                _logger.LogInformation($"Image uploaded: {blobName} for product {productId}");

                // Send a message to the Image Queue
                var queueMessage = new ImageQueueMessage
                {
                    ImageName = blobName,
                    ProductId = productId,
                    Action = "ResizeAndOptimize", 
                    Timestamp = DateTime.UtcNow
                };

                await _queueService.SendImageAsync(queueMessage);
                _logger.LogInformation($"Image processing job added to queue for {blobName}");

                return blobName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading image for product {productId}");
                throw;
            }
        }


        // Uploads multiple images for a product
        public async Task<List<string>> UploadMultipleImagesAsync(
            List<IFormFile> imageFiles,
            string productId)
        {
            var uploadedBlobs = new List<string>();

            foreach (var file in imageFiles)
            {
                if (file.Length > 0)
                {
                    var blobName = await UploadImageAsync(file, productId);
                    uploadedBlobs.Add(blobName);
                }
            }

            return uploadedBlobs;
        }

        // Gets the public URL for an image
        // Gets the public URL for an image (or SAS URL if private)
        public string GetImageUrl(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Generate a temporary SAS token to view it
            if (_containerClient.CanGenerateSasUri)
            {
                // Generate a SAS token that is valid for 1 hour
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _settings.BlobContainer,
                    BlobName = blobName,
                    Resource = "b", // 'b' stands for Blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            // Fallback to normal public URL if the container is public
            return blobClient.Uri.ToString();
        }

        // Gets multiple image URLs from blob names
        public List<string> GetImageUrls(List<string> blobNames)
        {
            var urls = new List<string>();
            foreach (var blobName in blobNames)
            {
                if (!string.IsNullOrWhiteSpace(blobName))
                {
                    urls.Add(GetImageUrl(blobName));
                }
            }
            return urls;
        }

        // Lists all images in the container with optional prefix
        public async Task<List<BlobInfo>> ListProductImagesAsync(string prefix = "")
        {
            var images = new List<BlobInfo>();

            try
            {
                // Use GetBlobsAsync with all required parameters including CancellationToken
                await foreach (var blobItem in _containerClient.GetBlobsAsync(
                    traits: BlobTraits.None,
                    states: BlobStates.None,
                    prefix: prefix,
                    cancellationToken: CancellationToken.None))
                {
                    images.Add(new BlobInfo
                    {
                        Name = blobItem.Name,
                        Url = GetImageUrl(blobItem.Name),
                        CreatedOn = blobItem.Properties.CreatedOn,
                        Size = blobItem.Properties.ContentLength ?? 0
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing images");
            }

            return images;
        }

        // Downloads an image as a stream
        public async Task<Stream> DownloadImageAsync(string blobName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                var response = await blobClient.DownloadStreamingAsync();
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading image: {blobName}");
                throw;
            }
        }

        // Deletes an image from Azure Blob Storage
        public async Task<bool> DeleteImageAsync(string blobName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(blobName);
                var result = await blobClient.DeleteIfExistsAsync();

                if (result)
                {
                    _logger.LogInformation($"Image deleted: {blobName}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting image: {blobName}");
                return false;
            }
        }

        // Deletes all images for a product
        public async Task<bool> DeleteProductImagesAsync(string productId)
        {
            try
            {
                var prefix = $"{productId}/";
                var deleted = false;

                await foreach (var blobItem in _containerClient.GetBlobsAsync(
                    traits: BlobTraits.None,
                    states: BlobStates.None,
                    prefix: prefix,
                    cancellationToken: CancellationToken.None))
                {
                    var blobClient = _containerClient
                        .GetBlobClient(blobItem.Name);
                    await blobClient.DeleteIfExistsAsync();
                    deleted = true;
                }

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error deleting images for product {productId}");
                return false;
            }
        }
    }

    public class BlobInfo
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public long Size { get; set; }
    }
}