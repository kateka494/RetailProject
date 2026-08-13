using System.ComponentModel.DataAnnotations;

namespace RetailProject.Models
{
    public class Customer
    {

        public string PartitionKey { get; set; } = "Customer";
        public string RowKey { get; set; }

        public string CustomerId { get; set; }

        [Required]
        [StringLength(60)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(60)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string EmailAddress { get; set; }

        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsLockedOut { get; set; } = false;
        public int LoginAttempts { get; set; }
        public DateTime LastLoginDate { get; set; }

        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

        public string UserPreferences { get; set; }
        public string PreferredCategories { get; set; }
    }
}