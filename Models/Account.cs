using System.ComponentModel.DataAnnotations;

namespace RetailProject.Models
{
    // For registration
    public class RegisterForm
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string City { get; set; }

        [Required, StringLength(50, MinimumLength = 6), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    // Used when a customer logs in
    public class LoginForm
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
    // Used to display customer profile information
    public class ProfileView
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public DateTime MemberSince { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }

    // Used when a customer edits their profile
    public class ProfileEditForm
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        public string Phone { get; set; }
        public string City { get; set; }
    }
}