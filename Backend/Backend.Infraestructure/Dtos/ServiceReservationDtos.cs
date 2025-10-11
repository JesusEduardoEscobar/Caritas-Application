// Backend.Infraestructure/Dtos/ServiceReservationDtos.cs
namespace Backend.Infrastructure.Dtos
{
    public class ServiceReservationCreateDto
    {
        public int UserId { get; set; }
        public int ShelterId { get; set; }
        public int ServiceId { get; set; }
        public string QrData { get; set; } = string.Empty;
    }

    public class ServiceReservationValidateDto
    {
        public string QrData { get; set; } = string.Empty;
    }
}
