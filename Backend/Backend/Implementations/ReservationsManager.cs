using Backend.Dtos;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ReservationsManager : IReservations
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<ReservationsManager> _logger;
        private const double _hoursBetweenReservations = 2;
        private const double _daysBetweenGetReservation = 3;

        public ReservationsManager(NeonTechDbContext context, ILogger<ReservationsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Reservation>>> GetReservations(int? shelterId = null, int? userId = null, ReservationStatus? status = null, DateTime? date = null)
        {
            try
            {
                var query = _context.Reservations
                    .Join(_context.Beds, r => r.BedId, b => b.Id, (r, b) => new { r, b })
                    .Join(_context.Shelters, rb => rb.b.ShelterId, s => s.Id, (rb, s) => new
                    {
                        rb.r.Id,
                        rb.r.UserId,
                        rb.r.BedId,
                        ShelterId = s.Id,
                        rb.r.StartDate,
                        rb.r.EndDate,
                        rb.r.Status,
                        rb.r.CreatedAt,
                        rb.r.QrData
                    }).AsQueryable();

                if (shelterId.HasValue)
                    query = query.Where(r => r.ShelterId == shelterId.Value);

                if (userId.HasValue)
                    query = query.Where(r => r.UserId == userId.Value);

                if (status.HasValue)
                    query = query.Where(r => r.Status == status.Value);

                if (date.HasValue)
                    query = query.Where(r =>
                        (r.Status == ReservationStatus.reserved || r.Status == ReservationStatus.checked_in)
                        && (date.Value.Date < r.EndDate) && (date.Value.Date.AddDays(_daysBetweenGetReservation) > r.StartDate)
                    );

                var reservations = await query
                    .Select(r => new Reservation
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        BedId = r.BedId,
                        StartDate = r.StartDate,
                        EndDate = r.EndDate,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt,
                        QrData = r.QrData
                    }).ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} reservaciones correctamente.", reservations.Count);
                return GlobalResponse<IEnumerable<Reservation>>.Success(reservations, reservations.Count, "Obtención de Reservaciones exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservaciones.");
                return GlobalResponse<IEnumerable<Reservation>>.Fault("Error al procesar Reservaciones", "-1", null);
            }
        }

        public async Task<GlobalResponse<Reservation>> GetReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada.", id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                _logger.LogInformation("Reservacion {Id} obtenida correctamente.", id);
                return GlobalResponse<Reservation>.Success(reservation, 1, "Obtención de Reservacion exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Reservacion {Id}.", id);
                return GlobalResponse<Reservation>.Fault("Error al procesar Reservaciones", "-1", null);
            }
        }

        public async Task<GlobalResponse<Reservation>> GetReservation(string qrData)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Where(r => r.QrData == qrData)
                    .FirstOrDefaultAsync();

                if (reservation == null)
                {
                    _logger.LogWarning("Reservacion con QR {qrData} no encontrada.", qrData);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                _logger.LogInformation("Reservacion con QR {qrData} obtenida correctamente.", qrData);
                return GlobalResponse<Reservation>.Success(reservation, 1, "Obtención de Reservacion exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Reservacion con QR {qrData}.", qrData);
                return GlobalResponse<Reservation>.Fault("Error al procesar Reservaciones", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<Reservation>> CreateReservation(ReservationCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Intento de crear cama con datos nulos.");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos", "400", null);
                }

                if (dto.StartDate >= dto.EndDate)
                {
                    _logger.LogWarning("Intento de crear reservacion con fechas incorrectas. StartDate >= EndDate");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos, fechas incorrectas", "400", null);
                }

                bool userExists = await _context.Users
                    .AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("El UserId {UserId} no existe.", dto.UserId);
                    return GlobalResponse<Reservation>.Fault($"El usuario con ID {dto.UserId} no existe.", "404", null);
                }

                var availableBed = await FindAvailableBed(dto.ShelterId, dto.StartDate, dto.EndDate);

                if (availableBed == null)
                {
                    _logger.LogWarning("No hay camas disponibles en ShelterId {ShelterId}.", dto.ShelterId);
                    return GlobalResponse<Reservation>.Fault($"No hay camas disponibles en ShelterId {dto.ShelterId}.", "409", null);
                }

                var reservation = new Reservation
                {
                    UserId = dto.UserId,
                    BedId = availableBed.Id,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = ReservationStatus.reserved,
                    CreatedAt = DateTime.UtcNow,
                    QrData = $"BED-{availableBed.Id}-U-{dto.UserId}-{Guid.NewGuid()}"
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} creada correctamente.", reservation.Id);
                return GlobalResponse<Reservation>.Success(reservation, 1, "Reservacion creada exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reservacion.");
                return GlobalResponse<Reservation>.Fault("Error al crear reservacion", "-1", null);
            }
        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<Reservation>> UpdateReservationStatus(ReservationPatchStatusDto dto)
        {
            try
            {
                var existing = await _context.Reservations.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                existing.Status = dto.Status;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<Reservation>.Success(existing, 1, "Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservacion {Id}.", dto.Id);
                return GlobalResponse<Reservation>.Fault("Error al actualizar reservacion", "-1", null);
            }
        }

        public async Task<GlobalResponse<Reservation>> UpdateReservationPeriod(ReservationPatchPeriodDto dto)
        {
            try
            {
                if (dto.StartDate >= dto.EndDate)
                {
                    _logger.LogWarning("Intento de actualizar reservacion con fechas incorrectas. StartDate >= EndDate");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos, fechas incorrectas", "400", null);
                }
                var existing = await _context.Reservations.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                if(await HasDateConflict(dto.Id, existing.BedId, dto.StartDate, dto.EndDate))
                {
                    _logger.LogWarning("Conflicto de fechas al actualizar Reservacion {dto.Id}.", dto.Id);
                    return GlobalResponse<Reservation>.Fault($"Conflicto de fechas al actualizar Reservacion {dto.Id}, debe haber un umbral de {_hoursBetweenReservations} horas entre reservaciones del mismo cuarto", "409", null);
                }

                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<Reservation>.Success(existing, 1, "Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservacion {Id}.", dto.Id);
                return GlobalResponse<Reservation>.Fault("Error al actualizar reservacion", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<Reservation>> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para eliminar.", id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} eliminada correctamente.", id);
                return GlobalResponse<Reservation>.Success(reservation, 1, "Reservacion eliminada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reservacion {Id}.", id);
                return GlobalResponse<Reservation>.Fault("Error al eliminar reservacion", "-1", null);
            }
        }

        #endregion


        private async Task<bool> HasDateConflict(int reservationId, int bedId, DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .AnyAsync(r => r.BedId == bedId && r.Id != reservationId
                       && (r.Status == ReservationStatus.reserved || r.Status == ReservationStatus.checked_in)
                       && (startDate.AddHours(-_hoursBetweenReservations) < r.EndDate) && (endDate.AddHours(_hoursBetweenReservations) > r.StartDate)
                );
        }

        private async Task<Bed?> FindAvailableBed(int shelterId, DateTime startDate, DateTime endDate)
        {
            var availableBed = await _context.Beds
                .Where(b => b.ShelterId == shelterId)
                .GroupJoin( 
                    // Camas con reservas en conflicto
                    _context.Reservations.Where(r =>
                        (r.Status == ReservationStatus.reserved || r.Status == ReservationStatus.checked_in)
                        && (startDate.AddHours(-_hoursBetweenReservations) < r.EndDate) && (endDate.AddHours(_hoursBetweenReservations) > r.StartDate)
                    ),
                    b => b.Id,
                    r => r.BedId,
                    (b, reservations) => new { Bed = b, Reservations = reservations }
                )
                .Where(x => !x.Reservations.Any()) // solo camas sin reservas conflictivas
                .OrderBy(x => x.Bed.Id)
                .Select(x => x.Bed)
                .FirstOrDefaultAsync();

            return availableBed;
        }

    }
}