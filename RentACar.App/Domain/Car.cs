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
        public string Description { get; set; }
        [Column(TypeName = "decimal(12,3)")]
        public decimal RentPrice {  get; set; }
        public User Tenant { get; set; }
        public string TenantId { get; set; }
    }
}
