﻿namespace RentACar.App.Models.Rents
{
    public class RentDetailsViewModel
    {
        public string Id {  get; set; }
        public string CarId { get; set; }
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string TenantId {  get; set; }
        public string TenantFirstName { get; set; }
        public string TenantLastName { get; set; }
        public string TenantUserName { get; set; }
        public string StartDate {  get; set; }
        public string EndDate { get; set; }
    }
}
