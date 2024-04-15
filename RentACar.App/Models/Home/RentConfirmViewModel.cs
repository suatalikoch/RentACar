namespace RentACar.App.Models.Home
{
    public class RentConfirmViewModel
    {
        public string Brand { get; set; }
        public string Model {  get; set; }
        public string Year { get; set; }
        public string Passenger { get; set; }
        public string Description { get; set; }
        public string RentPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
