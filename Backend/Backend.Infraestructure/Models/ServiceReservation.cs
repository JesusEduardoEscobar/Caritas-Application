namespace Backend.Infraestructure.Models
{
    public class ServiceReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShelterId { get; set; }
        public int ServiceId { get; set; }
        public string QrData { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
