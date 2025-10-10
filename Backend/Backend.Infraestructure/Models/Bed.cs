namespace Backend.Infraestructure.Models
{
    public class Bed
    {
        public int Id { get; set; }
        public int ShelterId { get; set; }
        public int BedNumber { get; set; }
        public bool IsAvailable { get; set; }
    }


}
