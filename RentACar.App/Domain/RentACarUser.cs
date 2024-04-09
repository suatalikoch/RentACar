using Microsoft.AspNetCore.Identity;

namespace RentACar.App.Domain
{
    public class RentACarUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PIN { get; set; }
    }
}
