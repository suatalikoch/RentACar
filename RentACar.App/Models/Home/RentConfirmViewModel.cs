namespace RentACar.App.Models.Home
{
    public class RentConfirmViewModel
    {
        public string CarId { get; set; }
        public string TenantId { get; set; }
        public string Brand { get; set; }
        public string Model {  get; set; }
        public string Year { get; set; }
        public string Passenger { get; set; }
        public string Description { get; set; }
        public string RentPrice { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RentDuration { get; set; }
        public string RentTotal { get; set; }
    }
}
