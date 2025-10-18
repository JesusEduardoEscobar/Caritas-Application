using System.ComponentModel.DataAnnotations;
using Backend.Infraestructure.Models;

namespace Backend.Dtos
{
    public class TransportRequestCreateDto
    {
        [Required(ErrorMessage = "El User Id es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "La Ubicación de Origen es obligatorio")]
        public string PickupLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Ubicación de Destino es obligatorio")]
        public string DropoffLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de Peticion es obligatoria")]
        public DateTime RequestDate { get; set; }
    }

    public class TransportRequestPatchDto
    {
        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La Ubicación de Origen es obligatorio")]
        public string PickupLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "La Ubicación de Destino es obligatorio")]
        public string DropoffLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de Peticion es obligatoria")]
        public DateTime RequestDate { get; set; }
    }

    public class TransportRequestPatchStatusDto
    {
        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Status de Reserva es obligatorio")]
        public ReservationStatus Status { get; set; }
    }

}