using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Backend.Dtos
{
    public class ShelterCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        [PhoneNumber]
        public string Phone { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public int Occupancy { get; set; }
    }

    public class CoordinatesDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}

public class PhoneNumberAttribute : ValidationAttribute
{
    // Regex para teléfonos (10 dígitos, opcional +0-999)
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