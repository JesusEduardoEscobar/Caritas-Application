
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Infraestructure.Dtos
{
    public class BedReservationCreateDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        [Column("qr_data")]
        public string QrData { get; set; } = string.Empty;
    }

    public enum ReservationStatus
    {
        reserved,
        checked_in,
        completed,
        canceled
    }


}
