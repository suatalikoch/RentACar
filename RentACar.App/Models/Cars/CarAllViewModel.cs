using System.ComponentModel;

namespace RentACar.App.Models.Cars
{
    public class CarAllViewModel
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Year { get; set; }
        public string Passenger { get; set; }
        public string Description { get; set; }

        [DisplayName("Rent Price")]
        public string RentPrice { get; set; }
        public string Renter { get; set; }
    }
}
