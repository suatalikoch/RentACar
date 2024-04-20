using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.App.Domain
{
    public class Car
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Brand { get; set; }
        public string Model {  get; set; }
        public int Year { get; set; }
        public int Passenger { get; set; }
        public string ImageLink { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(12,2)")]
        public decimal RentPrice {  get; set; } 
    }
}
