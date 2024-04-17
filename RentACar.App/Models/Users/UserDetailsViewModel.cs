using System.ComponentModel;

namespace RentACar.App.Models.Users
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string PIN {  get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("Password Hash")]
        public string PasswordHash { get; set; }
    }
}
