using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class ShelterServiceCreateDto
    {
        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int ServiceId { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a cero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La disponibilidad es obligatorio")]
        public bool IsAvailable { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a cero")]
        public int Capacity { get; set; }
    }

    public class ShelterServiceUpdateDto
    {
        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "El Service ID es obligatorio")]
        public int ServiceId { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a cero")]
        public decimal? Price { get; set; }

        public bool? IsAvailable { get; set; }
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a cero")]
        public int? Capacity { get; set; }
    }
}