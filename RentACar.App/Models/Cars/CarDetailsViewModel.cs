using System.ComponentModel;

namespace RentACar.App.Models.Cars
{
    public class CarDetailsViewModel
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year {  get; set; }
        public int Passenger {  get; set; }
        public string Description { get; set; }
        [DisplayName("Rent Price")]
        public decimal RentPrice {  get; set; }
        public string Renter { get; set; }
        [DisplayName("Renter Id")]
        public string RenterId { get; set; }
    }
}
