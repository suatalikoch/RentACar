using System.ComponentModel.DataAnnotations;

namespace RentACar.App.Models.Users
{
    public class UserEditBindingModel
    {
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        public string LastName { get; set; }

        [Display(Name = "PIN")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid PIN format.")]
        public string PIN { get; set; }

        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }
    }
}
