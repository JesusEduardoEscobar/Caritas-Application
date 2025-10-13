using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ServicesManager : IServices
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<ServicesManager> _logger;

        public ServicesManager(NeonTechDbContext context, ILogger<ServicesManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Service>>> GetServices()
        {
            try
            {
                var services = await _context.Services.ToListAsync();
                if (services == null || !services.Any())
                {
                    _logger.LogWarning("No se encontraron services en la base de datos.");
                    return GlobalResponse<IEnumerable<Service>>.Fault("Servicios no encontrados", "404", null);
                }

                _logger.LogInformation("Se obtuvieron {Count} servicios correctamente.", services.Count);
                return GlobalResponse<IEnumerable<Service>>.Success(services, services.Count, "Obtención de Services exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener services.");
                return GlobalResponse<IEnumerable<Service>>.Fault("Error al procesar autenticación: " + ex.Message, "-1", null);
            }
        }

        public async Task<GlobalResponse<Service>> GetService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado.", id);
                    return GlobalResponse<Service>.Fault("Shelter no encontrado", "404", null);
                }

                _logger.LogInformation("Service {Id} obtenido correctamente.", id);
                return GlobalResponse<Service>.Success(service, 1, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Service {Id}.", id);
                return GlobalResponse<Service>.Fault("Error al procesar autenticación: " + ex.Message, "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<Service>> CreateService(ServiceCreateDto serviceDto)
        {
            try
            {
                if (serviceDto == null)
                {
                    _logger.LogWarning("Intento de crear service con datos nulos.");
                    return GlobalResponse<Service>.Fault("Datos inválidos", "400", null);
                }

                var service = new Service
                {
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    IconKey = serviceDto.IconKey,
                };

                _context.Services.Add(service);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service {Id} creado correctamente.", service.Id);
                return GlobalResponse<Service>.Success(service, 1, "Service creado exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear service.");
                return GlobalResponse<Service>.Fault("Error al crear service", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<Service>> UpdateService(ServiceUpdateDto serviceDto)
        {
            try
            {
                var existing = await _context.Services.FindAsync(serviceDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Service {Id} no encontrado para actualizar.", serviceDto.Id);
                    return GlobalResponse<Service>.Fault("Service no encontrado", "404", null);
                }

                if (!string.IsNullOrWhiteSpace(serviceDto.Name)) existing.Name = serviceDto.Name;
                if (serviceDto.Description != null) existing.Description = serviceDto.Description;
                if (serviceDto.IconKey != null) existing.IconKey = serviceDto.IconKey;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service {Id} actualizado correctamente.", serviceDto.Id);
                return GlobalResponse<Service>.Success(existing, 1, "Service actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar service {Id}.", serviceDto.Id);
                return GlobalResponse<Service>.Fault("Error al actualizar service", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<Service>> DeleteService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service {Id} no encontrado para eliminar.", id);
                    return GlobalResponse<Service>.Fault("Service no encontrado", "404", null);
                }

                _context.Services.Remove(service);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service {Id} eliminado correctamente.", id);
                return GlobalResponse<Service>.Success(service, 1, "Service eliminado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar service {Id}.", id);
                return GlobalResponse<Service>.Fault("Error al eliminar service", "-1", null);
            }
        }

        #endregion

    }
}