using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace Backend.Implementations
{
    public class SheltersManager : IShelters
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<SheltersManager> _logger;

        public SheltersManager(NeonTechDbContext context, ILogger<SheltersManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Shelter>>> GetShelters()
        {
            try
            {
                var shelters = await _context.Shelters.ToListAsync();
                if (shelters == null || !shelters.Any())
                {
                    _logger.LogWarning("No se encontraron shelters en la base de datos.");
                    return GlobalResponse<IEnumerable<Shelter>>.Fault("Shelters no encontrados", "404", null);
                }

                _logger.LogInformation("Se obtuvieron {Count} shelters correctamente.", shelters.Count);
                return GlobalResponse<IEnumerable<Shelter>>.Success(shelters, shelters.Count, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener shelters.");
                return GlobalResponse<IEnumerable<Shelter>>.Fault("Error al procesar Shelters", "-1", null);
            }
        }

        public async Task<GlobalResponse<Shelter>> GetShelter(int id)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                _logger.LogInformation("Shelter {Id} obtenido correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Obtención de Shelter exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al procesar Shelters", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<Shelter>> CreateShelter(ShelterCreateDto shelterDto)
        {
            try
            {
                if (shelterDto == null)
                {
                    _logger.LogWarning("Intento de crear shelter con datos nulos.");
                    return GlobalResponse<Shelter>.Fault("Datos inválidos", "400", null);
                }

                var shelter = new Shelter
                {
                    Name = shelterDto.Name,
                    Address = shelterDto.Address,
                    Latitude = shelterDto.Latitude,
                    Longitude = shelterDto.Longitude,
                    Phone = shelterDto.Phone,
                    Capacity = shelterDto.Capacity,
                    Description = shelterDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    Occupancy = 0
                };

                _context.Shelters.Add(shelter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Shelter {Id} creado correctamente.", shelter.Id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Shelter creado exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear shelter.");
                return GlobalResponse<Shelter>.Fault("Error al crear shelter", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<Shelter>> UpdateShelter(ShelterUpdateDto shelterDto)
        {
            try
            {
                var existing = await _context.Shelters.FindAsync(shelterDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar.", shelterDto.Id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                if (!string.IsNullOrWhiteSpace(shelterDto.Name)) existing.Name = shelterDto.Name;
                if (!string.IsNullOrWhiteSpace(shelterDto.Address)) existing.Address = shelterDto.Address;
                if (!string.IsNullOrWhiteSpace(shelterDto.Phone)) existing.Phone = shelterDto.Phone;
                if (shelterDto.Capacity.HasValue) existing.Capacity = shelterDto.Capacity.Value;
                if (shelterDto.Latitude.HasValue) existing.Latitude = shelterDto.Latitude.Value;
                if (shelterDto.Longitude.HasValue) existing.Longitude = shelterDto.Longitude.Value;
                if (shelterDto.Description != null) existing.Description = shelterDto.Description;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Shelter {Id} actualizado correctamente.", shelterDto.Id);
                return GlobalResponse<Shelter>.Success(existing, 1, "Shelter actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar shelter {Id}.", shelterDto.Id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar shelter", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<Shelter>> DeleteShelter(int id)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para eliminar.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                _context.Shelters.Remove(shelter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Shelter {Id} eliminado correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Shelter eliminado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al eliminar shelter", "-1", null);
            }
        }

        #endregion

    }
}