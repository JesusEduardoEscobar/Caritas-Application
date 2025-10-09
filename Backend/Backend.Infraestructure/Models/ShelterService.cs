namespace Backend.Infraestructure.Models
{
    public class ShelterService
    {
        public int ShelterId { get; set; }
        public int ServiceId { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? Description { get; set; }
        public int Capacity { get; set; }
    }


}
