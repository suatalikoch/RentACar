using System.ComponentModel;

namespace RentACar.App.Models.Rents
{
    public class RentDetailsViewModel
    {
        public string Id {  get; set; }
        public string CarId { get; set; }
        [DisplayName("Car Brand")]
        public string CarBrand { get; set; }
        [DisplayName("Car Model")]
        public string CarModel { get; set; }
        public string TenantId {  get; set; }
        [DisplayName("Tenant First Name")]
        public string TenantFirstName { get; set; }
        [DisplayName("Tenant Last Name")]
        public string TenantLastName { get; set; }
        [DisplayName("Tenant Username")]
        public string TenantUserName { get; set; }
        [DisplayName("Start Date")]
        public string StartDate {  get; set; }
        [DisplayName("End Date")]
        public string EndDate { get; set; }
    }
}
