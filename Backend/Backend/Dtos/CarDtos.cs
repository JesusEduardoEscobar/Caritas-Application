using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    public class CarCreateDto
    {
        [Required(ErrorMessage = "El Shelter ID es obligatorio")]
        public int ShelterId { get; set; }

        [Required(ErrorMessage = "La placa del carro es obligatoria")]
        public string Plate { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo del carro es obligatorio")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "La disponibilidad es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor o igual a 0")]
        public int Capacity { get; set; }
    }

    public class CarUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        public int? ShelterId { get; set; }
        public string? Plate { get; set; }
        public string? Model { get; set; }
        public int? Capacity { get; set; }
    }

}