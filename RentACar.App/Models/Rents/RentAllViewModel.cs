﻿namespace RentACar.App.Models.Rents
{
    public class RentAllViewModel
    {
        public string Id { get; set; }
        public string CarId { get; set; }
        public string TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Approved { get;set; }
    }
}