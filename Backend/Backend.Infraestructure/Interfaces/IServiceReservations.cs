// Backend.Infraestructure/Interfaces/IServiceReservations.cs
using Backend.Infrastructure.Dtos;
using Backend.Infraestructure.Implementations; // si tu GlobalResponse está aquí

namespace Backend.Infraestructure.Interfaces
{
    public interface IServiceReservations
    {
        // Crear reserva con QR usando DTO
        Task<GlobalResponse<dynamic>> CreateReservation(ServiceReservationCreateDto dto);

        // Validar QR en base de datos
        Task<GlobalResponse<dynamic>> ValidateQr(string qrData);
    }
}
