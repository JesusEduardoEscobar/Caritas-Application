using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Database;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ReservationsManager : IReservations
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<ReservationsManager> _logger;

        public ReservationsManager(NeonTechDbContext context, ILogger<ReservationsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Reservation>>> GetReservations(int? shelterId = null, int? userId = null, ReservationStatus? status = null)
        {
            try
            {
                var query = _context.Reservations
                    .Join(_context.Beds, r => r.BedId, b => b.Id, (r, b) => new { r, b })
                    .Join(_context.Shelters, rb => rb.b.ShelterId, s => s.Id, (rb, s) => new ReservationQueryDto
                    {
                        Id = rb.r.Id,
                        UserId = rb.r.UserId,
                        BedId = rb.r.BedId,
                        ShelterId = s.Id,
                        StartDate = rb.r.StartDate,
                        EndDate = rb.r.EndDate,
                        Status = rb.r.Status,
                        CreatedAt = rb.r.CreatedAt
                    }).AsQueryable();

                if (shelterId.HasValue)
                    query = query.Where(r => r.ShelterId == shelterId.Value);

                if (userId.HasValue)
                    query = query.Where(r => r.UserId == userId.Value);

                if (status.HasValue)
                    query = query.Where(r => r.Status == status.Value);

                var reservations = await query
                    .Select(r => new Reservation
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        BedId = r.BedId,
                        StartDate = r.StartDate,
                        EndDate = r.EndDate,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt
                    }).ToListAsync();

                if (reservations == null || !reservations.Any())
                {
                    _logger.LogWarning("No se encontraron reservaciones en la base de datos.");
                    return GlobalResponse<IEnumerable<Reservation>>.Fault("Reservaciones no encontradas", "404", null);
                }

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

        #endregion

        #region POST

        public async Task<GlobalResponse<Reservation>> CreateReservation(ReservationCreateDto reservationDto)
        {
            try
            {
                if (reservationDto == null)
                {
                    _logger.LogWarning("Intento de crear cama con datos nulos.");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos", "400", null);
                }

                bool userExists = await _context.Users
                    .AnyAsync(u => u.Id == reservationDto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("El UserId {UserId} no existe.", reservationDto.UserId);
                    return GlobalResponse<Reservation>.Fault($"El usuario con ID {reservationDto.UserId} no existe.", "404", null);
                }

                bool bedExists = await _context.Beds
                    .AnyAsync(b => b.Id == reservationDto.BedId);
                if (!bedExists)
                {
                    _logger.LogWarning("El BedId {BedId} no existe.", reservationDto.BedId);
                    return GlobalResponse<Reservation>.Fault($"La cama con ID {reservationDto.BedId} no existe.", "404", null);
                }

                var reservation = new Reservation
                {
                    UserId = reservationDto.UserId,
                    BedId = reservationDto.BedId,
                    StartDate = reservationDto.StartDate,
                    EndDate = reservationDto.EndDate,
                    Status = ReservationStatus.reserved,
                    CreatedAt = DateTime.UtcNow
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

        #region PUT

        public async Task<GlobalResponse<Reservation>> UpdateReservation(ReservationUpdateDto reservationDto)
        {
            try
            {
                if (reservationDto == null)
                {
                    _logger.LogWarning("Intento de actualizar reservacion con datos nulos.");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos", "400", null);
                }

                if (reservationDto.BedId != null)
                {
                    bool bedExists = await _context.Beds
                        .AnyAsync(b => b.Id == reservationDto.BedId);
                    if (!bedExists)
                    {
                        _logger.LogWarning("El BedId {BedId} no existe.", reservationDto.BedId);
                        return GlobalResponse<Reservation>.Fault($"La cama con ID {reservationDto.BedId} no existe.", "404", null);
                    }
                }

                var existing = await _context.Reservations.FindAsync(reservationDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para actualizar.", reservationDto.Id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                if (reservationDto.BedId.HasValue) existing.BedId = reservationDto.BedId.Value;
                if (reservationDto.StartDate.HasValue) existing.StartDate = reservationDto.StartDate.Value;
                if (reservationDto.EndDate.HasValue) existing.EndDate = reservationDto.EndDate.Value;
                if (reservationDto.Status.HasValue) existing.Status = reservationDto.Status.Value;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} actualizada correctamente.", reservationDto.Id);
                return GlobalResponse<Reservation>.Success(existing, 1, "Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservacion {Id}.", reservationDto.Id);
                return GlobalResponse<Reservation>.Fault("Error al actualizar reservacion", "-1", null);
            }
        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<Reservation>> UpdateReservationStatus(ReservationUpdateStatusDto reservationDto)
        {
            try
            {
                var existing = await _context.Reservations.FindAsync(reservationDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para actualizar.", reservationDto.Id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                existing.Status = reservationDto.Status;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} actualizada correctamente.", reservationDto.Id);
                return GlobalResponse<Reservation>.Success(existing, 1, "Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservacion {Id}.", reservationDto.Id);
                return GlobalResponse<Reservation>.Fault("Error al actualizar reservacion", "-1", null);
            }
        }

        public async Task<GlobalResponse<Reservation>> UpdateReservationPeriod(ReservationUpdatePeriodDto reservationDto)
        {
            try
            {
                if (reservationDto.StartDate > reservationDto.EndDate)
                {
                    _logger.LogWarning("Intento de actualizar reservacion con fechas incorrectas.");
                    return GlobalResponse<Reservation>.Fault("Datos inválidos, fechas incorrectas", "400", null);
                }
                var existing = await _context.Reservations.FindAsync(reservationDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Reservacion {Id} no encontrada para actualizar.", reservationDto.Id);
                    return GlobalResponse<Reservation>.Fault("Reservacion no encontrada", "404", null);
                }

                existing.StartDate = reservationDto.StartDate;
                existing.EndDate = reservationDto.EndDate;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reservacion {Id} actualizada correctamente.", reservationDto.Id);
                return GlobalResponse<Reservation>.Success(existing, 1, "Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reservacion {Id}.", reservationDto.Id);
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

    }
}