using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace RetailProject.Models
{
   
    public class Product : ITableEntity
    {
        // Azure Table Storage properties
        public string PartitionKey { get; set; } = "Product";
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Product information
        public string ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string ProductDescription { get; set; }

        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCategory { get; set; }

        // References to images stored in Azure Blob Storage
        public string ImageBlobNames { get; set; }
        public string ImageUrls { get; set; }

        // Product management
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}