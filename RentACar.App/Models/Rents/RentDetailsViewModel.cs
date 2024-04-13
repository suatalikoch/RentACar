namespace RentACar.App.Models.Rents
{
    public class RentDetailsViewModel
    {
        public string Id {  get; set; }
        public string CarId { get; set; }
        public string TenantId {  get; set; }
        public string StartDate {  get; set; }
        public string EndDate { get; set; }
        public string Approved { get; set; }
    }
}
