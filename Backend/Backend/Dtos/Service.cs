namespace Backend.Dtos
{
    public class ServiceCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconKey { get; set; }
    }
}