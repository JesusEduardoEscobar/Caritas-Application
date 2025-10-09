namespace Backend.Infraestructure.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? Age { get; set; }
        //public int EconomicLevel { get; set; }
        public bool verificate { get; set; }
        //public string shelter { get; set; }
        public bool isAdmin { get; set; } = false;

    }


}
