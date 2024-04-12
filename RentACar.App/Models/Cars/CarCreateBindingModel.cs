﻿using System.ComponentModel.DataAnnotations;

namespace RentACar.App.Models.Cars
{
    public class CarCreateBindingModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Brand must contain only alphabetic characters.")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Car year must be a positive number.")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Car passenger seats must be a positive number.")]
        [Display(Name = "Passenger")]
        public int Passenger { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "Rent price must be a positive number.")]
        [Display(Name = "RentPrice")]
        public decimal RentPrice { get; set; }
    }
}