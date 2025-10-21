    namespace Backend.Infraestructure.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public UserRole Role { get; set; }
        public int? ShelterId { get; set; }
        public EconomicLevel EconomicLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Verified { get; set; }
    }


}