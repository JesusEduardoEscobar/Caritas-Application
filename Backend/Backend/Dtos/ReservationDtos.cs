using Backend.Infraestructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    public class ReservationCreateDto
    {
        [Required(ErrorMessage = "El User ID es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateTime EndDate { get; set; }
    }

    public class ReservationPatchStatusDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ReservationStatus es obligatoria")]
        public ReservationStatus Status { get; set; }
    }

    public class ReservationPatchPeriodDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateTime EndDate { get; set; }
    }


}