using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.App.Domain
{
    public class Rent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string CarId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public User Tenant { get; set; }
        public string TenantId { get; set; }
    }
}
