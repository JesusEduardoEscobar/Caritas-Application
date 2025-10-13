using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    public class BedCreateDto
    {
        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "El número de cama es obligatorio")]
        public string BedNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "La disponibilidad es obligatorio")]
        public bool IsAvailable { get; set; }
    }

    public class BedUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        public int? ShelterId { get; set; }
        public string? BedNumber { get; set; }
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