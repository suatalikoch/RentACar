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
        public decimal RentPrice {  get; set; }
        public string Renter { get; set; }
        public string RenterId { get; set; }
    }
}
