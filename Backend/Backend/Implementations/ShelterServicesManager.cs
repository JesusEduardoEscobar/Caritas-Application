using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Database;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ShelterServicesManager : IShelterServices
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<ShelterServicesManager> _logger;

        public ShelterServicesManager(NeonTechDbContext context, ILogger<ShelterServicesManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<ShelterService>>> GetShelterServices()
        {
            try
            {
                var shelterServices = await _context.ShelterServices.ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} shelterServices correctamente.", shelterServices.Count);
                return GlobalResponse<IEnumerable<ShelterService>>.Success(shelterServices, shelterServices.Count, "Obtención de ShelterServices exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener shelterServices.");
                return GlobalResponse<IEnumerable<ShelterService>>.Fault("Error al procesar ShelterServices", "-1", null);
            }
        }

        public async Task<GlobalResponse<IEnumerable<ShelterService>>> GetShelterServicesByShelter(int shelterId)
        {
            try
            {
                var shelterServices = await _context.ShelterServices
                    .Where(ss => ss.ShelterId == shelterId)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} shelterServices correctamente.", shelterServices.Count);
                return GlobalResponse<IEnumerable<ShelterService>>.Success(shelterServices, shelterServices.Count, "Obtención de ShelterServices exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener shelterServices.");
                return GlobalResponse<IEnumerable<ShelterService>>.Fault("Error al procesar ShelterServices", "-1", null);
            }
        }

        public async Task<GlobalResponse<ShelterService>> GetShelterService(int shelterId, int serviceId)
        {
            try
            {
                var shelterService = await _context.ShelterServices
                    .FirstOrDefaultAsync(ss => ss.ShelterId == shelterId && ss.ServiceId == serviceId);

                if (shelterService == null)
                {
                    _logger.LogWarning("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' no encontrado.", shelterId, serviceId);
                    return GlobalResponse<ShelterService>.Fault("ShelterService no encontrado", "404", null);
                }

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' obtenido correctamente.", shelterId, serviceId);
                return GlobalResponse<ShelterService>.Success(shelterService, 1, "Obtención de ShelterService exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", shelterId, serviceId);
                return GlobalResponse<ShelterService>.Fault("Error al procesar ShelterServices", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<ShelterService>> CreateShelterService(ShelterServiceCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Intento de crear shelterService con datos nulos.");
                    return GlobalResponse<ShelterService>.Fault("Datos inválidos", "400", null);
                }

                bool shelterExists = await _context.Shelters
                    .AnyAsync(s => s.Id == dto.ShelterId);
                if (!shelterExists)
                {
                    _logger.LogWarning("El ShelterId {ShelterId} no existe.", dto.ShelterId);
                    return GlobalResponse<ShelterService>.Fault($"El refugio con ID {dto.ShelterId} no existe.", "404", null);
                }

                bool serviceExists = await _context.Services
                    .AnyAsync(s => s.Id == dto.ServiceId);
                if (!serviceExists)
                {
                    _logger.LogWarning("El ServiceId {ServiceId} no existe.", dto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault($"El servicio con ID {dto.ServiceId} no existe.", "404", null);
                }

                var exists = await _context.ShelterServices
                    .AnyAsync(ss => ss.ShelterId == dto.ShelterId && ss.ServiceId == dto.ServiceId);
                if (exists)
                {
                    _logger.LogWarning("Ya existe un ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", dto.ShelterId, dto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault("Ya existe un ShelterService con esas claves.", "409", null);
                }

                var shelterService = new ShelterService
                {
                    ShelterId = dto.ShelterId,
                    ServiceId = dto.ServiceId,
                    Price = dto.Price,
                    IsAvailable = dto.IsAvailable,
                    Description = dto.Description,
                    Capacity = dto.Capacity,
                };

                _context.ShelterServices.Add(shelterService);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' creado correctamente.", dto.ShelterId, dto.ServiceId);
                return GlobalResponse<ShelterService>.Success(shelterService, 1, "ShelterService creado exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", dto.ShelterId, dto.ServiceId);
                return GlobalResponse<ShelterService>.Fault("Error al crear ShelterServices", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<ShelterService>> UpdateShelterService(ShelterServicePutDto dto)
        {
            try
            {
                var existing = await _context.ShelterServices
                    .FirstOrDefaultAsync(ss => ss.ShelterId == dto.ShelterId && ss.ServiceId == dto.ServiceId);

                if (existing == null)
                {
                    _logger.LogWarning("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' no encontrado para actualizar.", dto.ShelterId, dto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault("ShelterService no encontrado", "404", null);
                }

                existing.Price = dto.Price;
                existing.IsAvailable = dto.IsAvailable;
                existing.Description = dto.Description;
                existing.Capacity = dto.Capacity;

                await _context.SaveChangesAsync();

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' obtenido correctamente.", dto.ShelterId, dto.ServiceId);
                return GlobalResponse<ShelterService>.Success(existing, 1, "ShelterService actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", dto.ShelterId, dto.ServiceId);
                return GlobalResponse<ShelterService>.Fault("Error al actualizar ShelterServices", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<ShelterService>> DeleteShelterService(int shelterId, int serviceId)
        {
            try
            {
                var shelterService = await _context.ShelterServices
                    .FirstOrDefaultAsync(ss => ss.ShelterId == shelterId && ss.ServiceId == serviceId);

                if (shelterService == null)
                {
                    _logger.LogWarning("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' no encontrado.", shelterId, serviceId);
                    return GlobalResponse<ShelterService>.Fault("ShelterService no encontrado", "404", null);
                }

                _context.ShelterServices.Remove(shelterService);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' obtenido correctamente.", shelterId, serviceId);
                return GlobalResponse<ShelterService>.Success(shelterService, 1, "ShelterService eliminado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", shelterId, serviceId);
                return GlobalResponse<ShelterService>.Fault("Error al eliminar ShelterServices", "-1", null);
            }
        }

        #endregion

    }
}