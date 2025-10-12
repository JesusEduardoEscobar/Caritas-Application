using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
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
                if (shelterServices == null || !shelterServices.Any())
                {
                    _logger.LogWarning("No se encontraron shelterServices en la base de datos.");
                    return GlobalResponse<IEnumerable<ShelterService>>.Fault("ShelterServicess no encontrados", "404", null);
                }

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

                if (shelterServices == null || shelterServices.Count == 0)
                {
                    _logger.LogWarning("No se encontraron shelterServices con shelterId {ShelterId} en la base de datos.", shelterId);
                    return GlobalResponse<IEnumerable<ShelterService>>.Fault("ShelterServices no encontrados", "404", null);
                }

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

        public async Task<GlobalResponse<ShelterService>> CreateShelterService(ShelterServiceCreateDto shelterServiceDto)
        {
            try
            {
                if (shelterServiceDto == null)
                {
                    _logger.LogWarning("Intento de crear shelterService con datos nulos.");
                    return GlobalResponse<ShelterService>.Fault("Datos inválidos", "400", null);
                }

                bool shelterExists = await _context.Shelters
                    .AnyAsync(s => s.Id == shelterServiceDto.ShelterId);
                if (!shelterExists)
                {
                    _logger.LogWarning("El ShelterId {ShelterId} no existe.", shelterServiceDto.ShelterId);
                    return GlobalResponse<ShelterService>.Fault($"El refugio con ID {shelterServiceDto.ShelterId} no existe.", "404", null);
                }

                bool serviceExists = await _context.Services
                    .AnyAsync(s => s.Id == shelterServiceDto.ServiceId);
                if (!serviceExists)
                {
                    _logger.LogWarning("El ServiceId {ServiceId} no existe.", shelterServiceDto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault($"El servicio con ID {shelterServiceDto.ServiceId} no existe.", "404", null);
                }

                var exists = await _context.ShelterServices
                    .AnyAsync(ss => ss.ShelterId == shelterServiceDto.ShelterId && ss.ServiceId == shelterServiceDto.ServiceId);
                if (exists)
                {
                    _logger.LogWarning("Ya existe un ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault("Ya existe un ShelterService con esas claves.", "409", null);
                }

                var shelterService = new ShelterService
                {
                    ShelterId = shelterServiceDto.ShelterId,
                    ServiceId = shelterServiceDto.ServiceId,
                    Price = shelterServiceDto.Price,
                    IsAvailable = shelterServiceDto.IsAvailable,
                    Description = shelterServiceDto.Description,
                    Capacity = shelterServiceDto.Capacity,
                };

                _context.ShelterServices.Add(shelterService);
                await _context.SaveChangesAsync();

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' creado correctamente.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
                return GlobalResponse<ShelterService>.Success(shelterService, 1, "ShelterService creado exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
                return GlobalResponse<ShelterService>.Fault("Error al crear ShelterServices", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<ShelterService>> UpdateShelterService(ShelterServiceUpdateDto shelterServiceDto)
        {
            try
            {
                var existing = await _context.ShelterServices
                    .FirstOrDefaultAsync(ss => ss.ShelterId == shelterServiceDto.ShelterId && ss.ServiceId == shelterServiceDto.ServiceId);

                if (existing == null)
                {
                    _logger.LogWarning("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' no encontrado para actualizar.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
                    return GlobalResponse<ShelterService>.Fault("ShelterService no encontrado", "404", null);
                }

                if (shelterServiceDto.Price.HasValue) existing.Price = shelterServiceDto.Price.Value;
                if (shelterServiceDto.IsAvailable.HasValue) existing.IsAvailable = shelterServiceDto.IsAvailable.Value;
                if (shelterServiceDto.Description != null) existing.Description = shelterServiceDto.Description;
                if (shelterServiceDto.Capacity.HasValue) existing.Capacity = shelterServiceDto.Capacity.Value;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}' obtenido correctamente.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
                return GlobalResponse<ShelterService>.Success(existing, 1, "ShelterService actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ShelterService 'ShelterId={ShelterId}' 'ServiceId={ServiceId}'.", shelterServiceDto.ShelterId, shelterServiceDto.ServiceId);
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