using Backend.Infraestructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    /// <summary>
    /// Usada para filtro interno, no uso externo. Eso tilin :v
    /// </summary>
    public class ReservationQueryDto  
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BedId { get; set; }
        public int ShelterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class ReservationCreateDto
    {
        [Required(ErrorMessage = "El User ID es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El Bed ID es obligatorio")]
        public int BedId { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateTime EndDate { get; set; }
    }

    public class ReservationUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        public int? BedId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReservationStatus? Status { get; set; }
    }

    public class ReservationUpdateStatusDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ReservationStatus es obligatoria")]
        public ReservationStatus Status { get; set; }
    }

    public class ReservationUpdatePeriodDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateTime EndDate { get; set; }
    }


}