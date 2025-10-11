namespace Backend.Infraestructure.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string? Phone { get; set; }
        public EconomicLevel EconomicLevel { get; set; }
        public bool Verified { get; set; }
        public int ShelterId { get; set; }
        public UserRole Role { get; set; }
    }

    public enum EconomicLevel
    {
        low,
        medium,
        high
    }

    public enum UserRole
    {
        user,
        admin
    }


}
