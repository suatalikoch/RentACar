﻿namespace RentACar.App.Models.Rents
{
    public class RentCreateViewModel
    {
        public string CarId { get; set; }
        public string TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}