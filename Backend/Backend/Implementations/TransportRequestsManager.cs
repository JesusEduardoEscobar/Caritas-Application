using Backend.Dtos;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class TransportRequestsManager : ITransportRequests
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<TransportRequestsManager> _logger;

        public TransportRequestsManager(NeonTechDbContext context, ILogger<TransportRequestsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET
        
        public async Task<GlobalResponse<IEnumerable<TransportRequest>>> GetTransportRequests(int? userId = null, int? shelterId = null, DateTime? requestDate = null, ReservationStatus? status = null)
        {
            try
            {
                var query = _context.TransportRequests
                    .Join(_context.Cars, tr => tr.CarId, c => c.Id, (tr, c) => new { tr, c })
                    .Join(_context.Shelters, trb => trb.c.ShelterId, s => s.Id, (trb, s) => new
                    {
                        trb.tr.Id,
                        trb.tr.UserId,
                        trb.tr.CarId,
                        ShelterId = s.Id,
                        trb.tr.PickupLocation,
                        trb.tr.DropoffLocation,
                        trb.tr.RequestDate,
                        trb.tr.Status
                    })
                    .AsQueryable();

                if (userId.HasValue)
                    query = query.Where(tr => tr.UserId == userId.Value);

                if (shelterId.HasValue)
                    query = query.Where(tr => tr.ShelterId == shelterId.Value);

                if (requestDate.HasValue)
                    query = query.Where(tr => requestDate.Value.Date < tr.RequestDate && tr.RequestDate < requestDate.Value.Date.AddDays(1));

                if (status.HasValue)
                    query = query.Where(tr => tr.Status == status.Value);

                var transportRequests = await query
                    .Select(tr => new TransportRequest
                    {
                        Id = tr.Id,
                        UserId = tr.UserId,
                        CarId = tr.CarId,
                        PickupLocation = tr.PickupLocation,
                        DropoffLocation = tr.DropoffLocation,
                        RequestDate = tr.RequestDate,
                        Status = tr.Status,
                    }).ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} peticiones de transporte correctamente.", transportRequests.Count);
                return GlobalResponse<IEnumerable<TransportRequest>>.Success(transportRequests, transportRequests.Count, "Obtención de Peticiones de Transporte exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener peticiones de transporte.");
                return GlobalResponse<IEnumerable<TransportRequest>>.Fault("Error al procesar Peticiones de Transporte", "-1", null);
            }
        }

        public async Task<GlobalResponse<TransportRequest>> GetTransportRequest(int id)
        {
            try
            {
                var transportRequest = await _context.TransportRequests.FindAsync(id);
                if (transportRequest == null)
                {
                    _logger.LogWarning("Peticion de Transporte {Id} no encontrada.", id);
                    return GlobalResponse<TransportRequest>.Fault("Peticion de Transporte no encontrada", "404", null);
                }

                _logger.LogInformation("Peticion de Transporte {Id} obtenida correctamente.", id);
                return GlobalResponse<TransportRequest>.Success(transportRequest, 1, "Obtención de Peticion de Transporte exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Peticion de Transporte {Id}.", id);
                return GlobalResponse<TransportRequest>.Fault("Error al procesar Peticion de Transporte", "-1", null);
            }
        }

        public async Task<GlobalResponse<TransportRequest>> GetTransportRequest(string qrData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<TransportRequest>> CreateTransportRequest(TransportRequestCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Intento de crear peticion de transporte con datos nulos.");
                    return GlobalResponse<TransportRequest>.Fault("Datos inválidos", "400", null);
                }

                if (dto.RequestDate <= DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Intento de crear peticion de transporte con una fecha anterior al día de hoy");
                    return GlobalResponse<TransportRequest>.Fault("Datos inválidos, fechas incorrectas", "400", null);
                }

                bool userExists = await _context.Users
                    .AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("El UserId {UserId} no existe.", dto.UserId);
                    return GlobalResponse<TransportRequest>.Fault($"El usuario con ID {dto.UserId} no existe.", "404", null);
                }

                var availableCar = await FindAvailableCar(dto.ShelterId, dto.RequestDate);

                if (availableCar == null)
                {
                    _logger.LogWarning("No hay carros disponibles en ShelterId {ShelterId}.", dto.ShelterId);
                    return GlobalResponse<TransportRequest>.Fault($"No hay carros disponibles en ShelterId {dto.ShelterId}.", "409", null);
                }

                var transportRequest = new TransportRequest
                {
                    UserId = dto.UserId,
                    CarId = availableCar.Id,
                    PickupLocation = dto.PickupLocation,
                    DropoffLocation = dto.DropoffLocation,
                    RequestDate = dto.RequestDate,
                    Status = ReservationStatus.reserved,
                };

                _context.TransportRequests.Add(transportRequest);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Peticion de Transporte {Id} creada correctamente.", transportRequest.Id);
                return GlobalResponse<TransportRequest>.Success(transportRequest, 1, "Peticion de Transporte creada exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear peticion de transporte.");
                return GlobalResponse<TransportRequest>.Fault("Error al crear peticion de transporte", "-1", null);
            }
        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<TransportRequest>> UpdateTransportRequest(TransportRequestPatchDto dto)
        {
            try
            {
                if (dto.RequestDate <= DateTime.UtcNow.Date)
                {
                    _logger.LogWarning("Intento de actualizar peticion de transporte con una fecha anterior al día de hoy");
                    return GlobalResponse<TransportRequest>.Fault("Datos inválidos, fechas incorrectas", "400", null);
                }

                var existing = await _context.TransportRequests.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Peticion de Transporte {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<TransportRequest>.Fault("Peticion de Transporte no encontrada", "404", null);
                }

                existing.PickupLocation = dto.PickupLocation;
                existing.DropoffLocation = dto.DropoffLocation;
                existing.RequestDate = dto.RequestDate;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Peticion de Transporte {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<TransportRequest>.Success(existing, 1, "Peticion de Transporte actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar Peticion de Transporte {Id}.", dto.Id);
                return GlobalResponse<TransportRequest>.Fault("Error al actualizar Peticion de Transporte", "-1", null);
            }
        }

        public async Task<GlobalResponse<TransportRequest>> UpdateTransportRequestStatus(TransportRequestPatchStatusDto dto)
        {
            try
            {
                var existing = await _context.TransportRequests.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Peticion de Transporte {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<TransportRequest>.Fault("Peticion de Transporte no encontrada", "404", null);
                }

                existing.Status = dto.Status;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Peticion de Transporte {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<TransportRequest>.Success(existing, 1, "Peticion de Transporte actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar Peticion de Transporte {Id}.", dto.Id);
                return GlobalResponse<TransportRequest>.Fault("Error al actualizar Peticion de Transporte", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<TransportRequest>> DeleteTransportRequest(int id)
        {
            try
            {
                var transportRequest = await _context.TransportRequests.FindAsync(id);
                if (transportRequest == null)
                {
                    _logger.LogWarning("Peticion de Transporte {Id} no encontrada para eliminar.", id);
                    return GlobalResponse<TransportRequest>.Fault("Peticion de Transporte no encontrada", "404", null);
                }

                _context.TransportRequests.Remove(transportRequest);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Peticion de Transporte {Id} eliminada correctamente.", id);
                return GlobalResponse<TransportRequest>.Success(transportRequest, 1, "Peticion de Transporte eliminada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar Peticion de Transporte {Id}.", id);
                return GlobalResponse<TransportRequest>.Fault("Error al eliminar Peticion de Transporte", "-1", null);
            }
        }

        #endregion



        // Cars with less petitions today - Greedy approach - No colition Detection
        private async Task<Car?> FindAvailableCar(int shelterId, DateTime requestDate)
        {
            var availableCar = await _context.Cars
                .Where(c => c.ShelterId == shelterId)
                .GroupJoin(
                    _context.TransportRequests.Where(tr =>
                        (tr.Status == ReservationStatus.reserved || tr.Status == ReservationStatus.checked_in)
                        && requestDate.Date < tr.RequestDate && tr.RequestDate < requestDate.Date.AddDays(1)
                    ),
                    c => c.Id,
                    tr => tr.CarId,
                    (c, tr) => new { Car = c, TransportRequests = tr}
                )
                .OrderBy(x => x.TransportRequests.Count())
                .Select(x => x.Car)
                .FirstOrDefaultAsync();

            return availableCar;
        }

    }
}