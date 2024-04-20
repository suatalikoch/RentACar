using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentACar.App.Models.Cars
{
    public class CarDetailsViewModel
    {
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year {  get; set; }
        public int Passenger {  get; set; }

        [Display(Name = "Image Link")]
        public string ImageLink { get; set; }
        public string Description { get; set; }
        [DisplayName("Rent Price")]
        public decimal RentPrice {  get; set; }
    }
}
