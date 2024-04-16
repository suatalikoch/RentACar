using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.App.Domain
{
    public class PendingRent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string CarId { get; set; }
        public string TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Approved { get; set; }
    }
}
