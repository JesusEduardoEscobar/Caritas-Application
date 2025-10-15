using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class TransportRequestCreateDto
    {
        [Required(ErrorMessage = "El User Id es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "La Ubicación de Origen es obligatorio")]
        public string PickupLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Ubicación de Destino es obligatorio")]
        public string DropoffLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public DateTime RequestDate { get; set; }
    }

    public class TransportRequestPatchDto
    {
        public string? PickupLocation { get; set; }
        public string? DropoffLocation { get; set; }
        public DateTime? RequestDate { get; set; }
    }

}