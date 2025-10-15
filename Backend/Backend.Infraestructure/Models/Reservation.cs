using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infraestructure.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("qr_data")]
        public string QrData { get; set; } = string.Empty;
    }


}
