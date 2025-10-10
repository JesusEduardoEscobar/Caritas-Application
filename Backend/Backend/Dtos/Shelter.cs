namespace Backend.Dtos
{
    public class ShelterCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public int Occupancy { get; set; }
    }

    public class CoordinatesDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}