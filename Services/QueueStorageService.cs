using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using RetailProject.Models;
using RetailProject.Options;

namespace RetailProject.Services
{
    // Handles messages sent to Azure Queue Storage
    public class QueueStorageService
    {
        private readonly QueueClient _orderQueue;
        private readonly QueueClient _inventoryQueue;
        private readonly QueueClient _imageQueue;

        public QueueStorageService(IOptions<AzureStorageOptions> settings)
        {
            var options = settings.Value;

            _orderQueue = new QueueClient(
                options.ConnectionString,
                options.OrderQueue);

            _inventoryQueue = new QueueClient(
                options.ConnectionString,
                options.InventoryQueue);

            _imageQueue = new QueueClient(
                options.ConnectionString,
                options.ImageQueue);
        }

        // Creates the queues if they do not already exist
        public async Task InitializeAsync()
        {
            await _orderQueue.CreateIfNotExistsAsync();
            await _inventoryQueue.CreateIfNotExistsAsync();
            await _imageQueue.CreateIfNotExistsAsync();
        }

        // Adds an order to the order processing queue
        public async Task SendOrderAsync(OrderQueueMessage message)
        {
            string data = JsonSerializer.Serialize(message);

            await _orderQueue.SendMessageAsync(data);
        }

        // Adds an inventory update to the inventory queue
        public async Task SendInventoryAsync(InventoryQueueMessage message)
        {
            string data = JsonSerializer.Serialize(message);

            await _inventoryQueue.SendMessageAsync(data);
        }

        // Adds an image task to the image processing queue
        public async Task SendImageAsync(ImageQueueMessage message)
        {
            string data = JsonSerializer.Serialize(message);

            await _imageQueue.SendMessageAsync(data);
        }
    }
}