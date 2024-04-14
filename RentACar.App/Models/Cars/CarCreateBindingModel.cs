﻿using System.ComponentModel.DataAnnotations;

namespace RentACar.App.Models.Cars
{
    public class CarCreateBindingModel
    {
        [Required]
        [Display(Name = "Brand")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Brand must contain only alphabetic characters.")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Car year must be a positive number.")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Passenger")]
        [Range(0, int.MaxValue, ErrorMessage = "Car passenger seats must be a positive number.")]
        public int Passenger { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "RentPrice")]
        [Range(0.00, double.MaxValue, ErrorMessage = "Rent price must be a positive number.")]
        public decimal RentPrice { get; set; }
    }
}
