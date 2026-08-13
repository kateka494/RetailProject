namespace RetailProject.Models
{
    // Represents an order in the application
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string OrderItems { get; set; }
        public decimal TotalPrice { get; set; }

        // Current status of the order
        // Examples: Received, Processing, Completed, Cancelled
        public string CurrentStatus { get; set; } = "Received";

        // Date and time the order was placed
        public DateTime OrderPlaced { get; set; } = DateTime.UtcNow;

        // Date and time the status was last updated
        public DateTime? StatusUpdated { get; set; }

        // Delivery information
        public string DeliveryAddress { get; set; }
        public string OrderNotes { get; set; }
    }
}