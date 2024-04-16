namespace RentACar.App.Models.Pending
{
    public class PendingRentAllViewModel
    {
        public string Id { get; set; }
        public string CarId { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string TenantId { get; set; }
        public string TenantFirstName { get; set; }
        public string TenantLastName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
