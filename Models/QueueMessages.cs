namespace RetailProject.Models
{
    // Message used when an order needs to be processed
    public class OrderQueueMessage
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public string Action { get; set; }

        public decimal Amount { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Message used when inventory needs to be updated
    public class InventoryQueueMessage
    {
        public string ProductId { get; set; }

        public string Action { get; set; }

        public int Quantity { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // Message used when an image needs processing
    public class ImageQueueMessage
    {
        public string ImageName { get; set; }

        public string ProductId { get; set; }

        public string Action { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}