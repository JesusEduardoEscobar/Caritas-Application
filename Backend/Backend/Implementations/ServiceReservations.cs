using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using Backend.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Backend.Implementations
{
    public class ServiceReservationsManager : IServiceReservations
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<ServiceReservationsManager> _logger;

        public ServiceReservationsManager(NeonTechDbContext context, ILogger<ServiceReservationsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<ServiceReservation>>> GetServiceReservations(int? userId = null, int? shelterId = null, int? serviceId = null, bool? isActive = null)
        {
            try
            {
                var query = _context.ServiceReservations.AsQueryable();

                if (userId.HasValue)
                    query = query.Where(sr => sr.UserId == userId.Value);

                if (shelterId.HasValue)
                    query = query.Where(sr => sr.ShelterId == shelterId.Value);

                if (serviceId.HasValue)
                    query = query.Where(sr => sr.ServiceId == serviceId.Value);

                if (isActive.HasValue)
                    query = query.Where(sr => sr.IsActive == isActive.Value);

                var serviceReservations = await query.ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} reservaciones de servicio correctamente.", serviceReservations.Count);
                return GlobalResponse<IEnumerable<ServiceReservation>>.Success(serviceReservations, serviceReservations.Count, "Obtención de Reservaciones de Servicio exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservaciones de servicio.");
                return GlobalResponse<IEnumerable<ServiceReservation>>.Fault("Error al procesar Reservaciones de Servicio", "-1", null);
            }
        }

        public async Task<GlobalResponse<ServiceReservation>> GetServiceReservation(int id)
        {
            try
            {
                var serviceReservation = await _context.ServiceReservations.FindAsync(id);
                if (serviceReservation == null)
                {
                    _logger.LogWarning("Reservacion de Servicio {Id} no encontrada.", id);
                    return GlobalResponse<ServiceReservation>.Fault("Reservacion de Servicio no encontrada", "404", null);
                }

                _logger.LogInformation("Reservacion de Servicio {Id} obtenida correctamente.", id);
                return GlobalResponse<ServiceReservation>.Success(serviceReservation, 1, "Obtención de Reservacion de Servicio exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Reservacion de Servicio {Id}.", id);
                return GlobalResponse<ServiceReservation>.Fault("Error al procesar Reservacion de Servicio", "-1", null);
            }
        }

        public async Task<GlobalResponse<ServiceReservation>> GetServiceReservation(string qrData)
        {
            try
            {
                var serviceReservation = await _context.ServiceReservations
                    .Where(sr => sr.QrData == qrData)
                    .FirstOrDefaultAsync();

                if (serviceReservation == null)
                {
                    _logger.LogWarning("Reservacion de Servicio con QR {qrData} no encontrada.", qrData);
                    return GlobalResponse<ServiceReservation>.Fault("Reservacion de Servicio no encontrada", "404", null);
                }

                _logger.LogInformation("Reservacion de Servicio con QR {qrData} obtenida correctamente.", qrData);
                return GlobalResponse<ServiceReservation>.Success(serviceReservation, 1, "Obtención de Reservacion de Servicio exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Reservacion de Servicio con QR {qrData}.", qrData);
                return GlobalResponse<ServiceReservation>.Fault("Error al procesar Reservacion de Servicio", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<ServiceReservation>> CreateServiceReservation(ServiceReservationCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Intento de crear reserva de servicio con datos nulos.");
                    return GlobalResponse<ServiceReservation>.Fault("Datos inválidos", "400", null);
                }

                bool userExists = await _context.Users
                    .AnyAsync(u => u.Id == dto.UserId);
                if (!userExists)
                {
                    _logger.LogWarning("El UserId {UserId} no existe.", dto.UserId);
                    return GlobalResponse<ServiceReservation>.Fault($"El usuario con ID {dto.UserId} no existe", "404", null);
                }

                var shelterService = await _context.ShelterServices
                    .Where(ss => ss.ShelterId == dto.ShelterId && ss.ServiceId == dto.ServiceId
                        && ss.IsAvailable == true
                    )
                    .FirstOrDefaultAsync();
                if (shelterService == null)
                {
                    _logger.LogWarning("No hay existe un ShelterService {ShelterId} {ServiceId}, o no esta Activo.", dto.ShelterId, dto.ServiceId);
                    return GlobalResponse<ServiceReservation>.Fault($"No hay existe un ShelterService {dto.ShelterId} {dto.ServiceId}, o no esta Activo", "404", null);
                }

                // Validate ShelterService Capacity and current ShelterDates
                var serviceReservationCount = await _context.ServiceReservations
                    .Where(sr => sr.ShelterId == dto.ShelterId && sr.ServiceId == dto.ServiceId
                        && sr.ServiceDate.Date == dto.ServiceDate.Date
                    )
                    .CountAsync();
                if (serviceReservationCount >= shelterService.Capacity)
                {
                    _logger.LogWarning("Capacidad máxima alcanzada en ShelterService {ShelterId} {ServiceId}.", dto.ShelterId, dto.ServiceId);
                    return GlobalResponse<ServiceReservation>.Fault($"Capacidad máxima alcanzada en ShelterService {dto.ShelterId} {dto.ServiceId}.", "409", null);
                }

                var serviceReservation = new ServiceReservation
                {
                    UserId = dto.UserId,
                    ShelterId = dto.ShelterId,
                    ServiceId = dto.ServiceId,
                    QrData = $"SERVICE-{dto.ServiceId}-SHELTER-{dto.ShelterId}-U-{dto.UserId}-{Guid.NewGuid()}",
                    ServiceDate = dto.ServiceDate,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ServiceReservations.Add(serviceReservation);
                await _context.SaveChangesAsync();

                return GlobalResponse<ServiceReservation>.Success(serviceReservation, 1, "Reserva de Servicio creada", "200");
            }

            catch (Exception ex)
            {
                return GlobalResponse<ServiceReservation>.Fault($"Error creando reserva de servicio: {ex.Message}", "-1", null);
            }

        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<ServiceReservation>> UpdateServiceReservationIsActive(ServiceReservationPatchIsActiveDto dto)
        {
            try
            {
                var existing = await _context.ServiceReservations.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Service Reservacion {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<ServiceReservation>.Fault("Service Reservacion no encontrada", "404", null);
                }

                existing.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service Reservacion {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<ServiceReservation>.Success(existing, 1, "Service Reservacion actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar Service Reservacion {Id}.", dto.Id);
                return GlobalResponse<ServiceReservation>.Fault("Error al actualizar Service Reservacion", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<ServiceReservation>> DeleteServiceReservation(int id)
        {
            try
            {
                var serviceReservation = await _context.ServiceReservations.FindAsync(id);
                if (serviceReservation == null)
                {
                    _logger.LogWarning("Service Reservacion {Id} no encontrada para eliminar.", id);
                    return GlobalResponse<ServiceReservation>.Fault("Service Reservacion no encontrada", "404", null);
                }

                _context.ServiceReservations.Remove(serviceReservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service Reservacion {Id} eliminada correctamente.", id);
                return GlobalResponse<ServiceReservation>.Success(serviceReservation, 1, "Service Reservacion eliminada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar Service Reservacion {Id}.", id);
                return GlobalResponse<ServiceReservation>.Fault("Error al eliminar Service Reservacion", "-1", null);
            }
        }

        #endregion

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
