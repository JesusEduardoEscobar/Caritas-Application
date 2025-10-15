using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class ServiceReservationCreateDto
    {
        [Required(ErrorMessage = "El User Id es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int ServiceId { get; set; }
    }

    public class ServiceReservationValidateDto
    {
        public string QrData { get; set; } = string.Empty;
    }

}