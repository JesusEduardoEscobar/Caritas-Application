namespace Backend.Infraestructure.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int ShelterId { get; set; }
        public string Plate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }


}
