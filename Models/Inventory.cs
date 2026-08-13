namespace RetailProject.Models
{
    // Represents product inventory in the application
    public class Inventory
    {
        public string InventoryId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        // Stock tracking
        public int QuantityOnHand { get; set; }
        public int ReservedQuantity { get; set; }

        // Calculates the quantity available for new orders
        public int AvailableQuantity => QuantityOnHand - ReservedQuantity;

        // Date and time the inventory was last updated
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Location where the stock is stored
        public string WarehouseLocation { get; set; }
    }
}