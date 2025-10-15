using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    public class ShelterCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Address { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "Latitud inválida")]
        public decimal Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitud inválida")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [PhoneNumber(ErrorMessage = "El teléfono es inválido")]
        public string Phone { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a cero")]
        public int Capacity { get; set; }

        public string? Description { get; set; }
    }

    public class ShelterPutDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string Address { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "Latitud inválida")]
        public decimal Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitud inválida")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [PhoneNumber(ErrorMessage = "El teléfono es inválido")]
        public string Phone { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor a cero")]
        public int Capacity { get; set; }

        public string? Description { get; set; }
    }


    public class PhoneNumberAttribute : ValidationAttribute
    {
        // Regex para teléfonos (10 dígitos, opcional +1-999)
        private static readonly Regex phoneRegex = new(@"^(\+([1-9]\d{0,2})\s?)?\d{10}$", RegexOptions.Compiled);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string phone)
            {
                if (!phoneRegex.IsMatch(phone))
                {
                    return new ValidationResult("El formato del número telefónico es inválido.");
                }
            }
            return ValidationResult.Success;
        }
    }
}

