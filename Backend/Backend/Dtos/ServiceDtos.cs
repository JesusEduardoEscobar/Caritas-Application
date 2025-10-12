using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    public class ServiceCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconKey { get; set; }
    }

    public class ServiceUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? IconKey { get; set; }
    }


}