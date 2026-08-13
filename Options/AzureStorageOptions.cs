namespace RetailProject.Options
{
    // Stores the Azure Storage configuration
    public class AzureStorageOptions
    {
        public string ConnectionString { get; set; }

        public string CustomersTable { get; set; } = "Customers";

        public string ProductsTable { get; set; } = "Products";

        public string BlobContainer { get; set; } = "retailmedia";

        public string OrderQueue { get; set; } = "order-processing";

        public string InventoryQueue { get; set; } = "inventory-processing";

        public string ImageQueue { get; set; } = "image-processing";

        public string LogFileShare { get; set; } = "application-logs";
    }
}