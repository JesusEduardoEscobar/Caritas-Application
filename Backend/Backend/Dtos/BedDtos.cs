using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class BedCreateDto
    {
        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor o igual a cero")]
        public int BedNumber { get; set; }

        [Required(ErrorMessage = "La disponibilidad es obligatorio")]
        public bool IsAvailable { get; set; }
    }

    public class BedUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        public int? ShelterId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor o igual a cero")]
        public int? BedNumber { get; set; }

        public bool? IsAvailable { get; set; }
    }

    public class BedUpdateAvailabilityDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La disponibilidad es obligatoria")]
        public bool IsAvailable { get; set; }
    }
}