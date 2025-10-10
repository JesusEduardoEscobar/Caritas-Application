namespace Backend.Infraestructure.Models
{
    public class TransportRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
        public string PickupLocation { get; set; } = string.Empty;
        public string DropoffLocation { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public ReservationStatus Status { get; set; }
    }


}
