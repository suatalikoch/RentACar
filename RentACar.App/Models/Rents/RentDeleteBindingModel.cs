namespace RentACar.App.Models.Rents
{
    public class RentDeleteBindingModel
    {
        public string Id { get; set; }
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string TenantFirstName { get; set; }
        public string TenantLastName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
