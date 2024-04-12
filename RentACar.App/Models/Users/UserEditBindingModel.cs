﻿using System.ComponentModel.DataAnnotations;

namespace RentACar.App.Models.Users
{
    public class UserEditBindingModel
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name must contain only alphabetic characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid PIN format.")]

        [Display(Name = "PIN")]
        public string PIN { get; set; }

        [RegularExpression(@"^\d{7,15}$", ErrorMessage = "Invalid phone number format.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
