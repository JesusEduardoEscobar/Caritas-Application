namespace Backend.Infraestructure.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconKey { get; set; }
    }


}
