﻿using RentACar.App.Domain;

namespace RentACar.App.Models.Home
{
    public class AvailableCarsViewModel
    {
        public List<Car> AvailableCars { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
