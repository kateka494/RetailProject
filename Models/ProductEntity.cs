using Azure;
using Azure.Data.Tables;

namespace RetailProject.Models
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Products"; 
        public string RowKey { get; set; } = Guid.NewGuid().ToString(); // Unique ID per product
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageBlobUrl { get; set; } = string.Empty; 
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}