using RentACar.App.Domain;

namespace RentACar.App.Models.Rents
{
    public class RentAllViewModel
    {
        public string CarId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public User Tenant { get; set; }
    }
}
