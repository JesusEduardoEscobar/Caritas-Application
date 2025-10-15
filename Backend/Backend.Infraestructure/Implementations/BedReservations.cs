using Backend.Infrastructure.Database;
using Backend.Infrastructure.Dtos;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Dtos;


public class BedReservations : IBedReservations
{
    private readonly NeonTechDbContext _context;

    public BedReservations(NeonTechDbContext context)
    {
        _context = context;
    }

    public async Task<GlobalResponse<dynamic>> CreateReservation(BedReservationCreateDto dto)
    {
        try
        {
            // validaciones
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists) return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

            var bedExists = await _context.Beds.AnyAsync(b => b.Id == dto.BedId);
            if (!bedExists) return GlobalResponse<dynamic>.Fault("Cama no encontrada", "404", null);

            // asegurar UTC
            var startUtc = dto.StartDate.Kind == DateTimeKind.Utc ? dto.StartDate : dto.StartDate.ToUniversalTime();
            var endUtc = dto.EndDate.Kind == DateTimeKind.Utc ? dto.EndDate : dto.EndDate.ToUniversalTime();

            // generar QrData si no viene
            var qrData = string.IsNullOrWhiteSpace(dto.QrData) ? $"BED-{dto.BedId}-U-{dto.UserId}-{Guid.NewGuid()}" : dto.QrData;

            var reservation = new Reservation
            {
                UserId = dto.UserId,
                BedId = dto.BedId,
                StartDate = startUtc,
                EndDate = endUtc,
                Status = (Backend.Infraestructure.Models.ReservationStatus)dto.Status,      // enum en C#
                CreatedAt = DateTime.UtcNow,
                QrData = qrData
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // devolver el id y qrData para el controller
            return GlobalResponse<dynamic>.Success(new { reservation.Id, reservation.QrData }, 1, "Reserva creada", "200");
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
            var reservation = await _context.Reservations
                .Where(r => r.QrData == qrData)
                .Select(r => new { r.Id, r.UserId, r.BedId, r.StartDate, r.EndDate, r.Status, r.CreatedAt })
                .FirstOrDefaultAsync();

            if (reservation == null) return GlobalResponse<dynamic>.Fault("QR no encontrado", "404", null);

            return GlobalResponse<dynamic>.Success(reservation, 1, "QR válido", "200");
        }
        catch (Exception ex)
        {
            return GlobalResponse<dynamic>.Fault($"Error validando QR: {ex.Message}", "-1", null);
        }
    }
}
