// Backend.Infraestructure/Implementations/ServiceReservations.cs
using Backend.Infrastructure.Database;
using Backend.Infrastructure.Dtos;
using Backend.Infraestructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infraestructure.Implementations
{
    public class ServiceReservations : IServiceReservations
    {
        private readonly NeonTechDbContext _context;

        public ServiceReservations(NeonTechDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalResponse<dynamic>> CreateReservation(ServiceReservationCreateDto dto)
        {
            try
            {
                // Opcional: validar que existan user/shelter/service
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
                if (!userExists) return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

                var shelterExists = await _context.Shelters.AnyAsync(s => s.Id == dto.ShelterId);
                if (!shelterExists) return GlobalResponse<dynamic>.Fault("Refugio no encontrado", "404", null);

                var serviceExists = await _context.Services.AnyAsync(s => s.Id == dto.ServiceId);
                if (!serviceExists) return GlobalResponse<dynamic>.Fault("Servicio no encontrado", "404", null);

                var reservation = new ServiceReservation
                {
                    UserId = dto.UserId,
                    ShelterId = dto.ShelterId,
                    ServiceId = dto.ServiceId,
                    QrData = dto.QrData,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ServiceReservations.Add(reservation);
                await _context.SaveChangesAsync();

                return GlobalResponse<dynamic>.Success(new { reservation.Id }, 1, "Reserva creada", "200");
            }

            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error creando reserva: {ex.Message}", "-1", null);
            }

        }

        public async Task<GlobalResponse<dynamic>> ValidateQr(string qrData)
        {
            try
            {
                var reservation = await _context.ServiceReservations
                    .Where(r => r.QrData == qrData && r.IsActive)
                    .Select(r => new { r.Id, r.UserId, r.ShelterId, r.ServiceId, r.CreatedAt })
                    .FirstOrDefaultAsync();

                if (reservation == null)
                    return GlobalResponse<dynamic>.Fault("QR no encontrado", "404", null);

                return GlobalResponse<dynamic>.Success(reservation, 1, "QR válido", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault("Error validando QR", "-1", null);
            }
        }
    }
}
