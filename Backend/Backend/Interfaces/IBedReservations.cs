// Backend.Infraestructure/Interfaces/IServiceReservations.cs
using Backend.Infrastructure.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Dtos; // si tu GlobalResponse está aquí

namespace Backend.Interfaces
{
    public interface IBedReservations
    {
        // Crear reserva con QR usando DTO
        Task<GlobalResponse<dynamic>> CreateReservation(BedReservationCreateDto dto);

        // Validar QR en base de datos
        Task<GlobalResponse<dynamic>> ValidateQr(string qrData);
    }
}