namespace Backend.Infraestructure.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public int ShelterId { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }


}
