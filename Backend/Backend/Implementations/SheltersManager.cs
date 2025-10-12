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
                    return GlobalResponse<IEnumerable<Shelter>>.Fault("Shelter no encontrado", "404", null);
                }

                _logger.LogInformation("Se obtuvieron {Count} shelters correctamente.", shelters.Count);
                return GlobalResponse<IEnumerable<Shelter>>.Success(shelters, shelters.Count, "Obtención de Shelters exitosa", "200");
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

        public async Task<GlobalResponse<Shelter>> CreateShelter(Shelter shelter)
        {
            try
            {
                if (shelter == null)
                {
                    _logger.LogWarning("Intento de crear shelter con datos nulos.");
                    return GlobalResponse<Shelter>.Fault("Datos inválidos", "400", null);
                }

                shelter.CreatedAt = DateTime.UtcNow;
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

        public async Task<GlobalResponse<Shelter>> UpdateShelter(Shelter shelter)
        {
            try
            {
                var existing = await _context.Shelters.FindAsync(shelter.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar.", shelter.Id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }   

                _context.Entry(existing).CurrentValues.SetValues(shelter);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Shelter {Id} actualizado correctamente.", shelter.Id);
                return GlobalResponse<Shelter>.Success(existing, 1, "Shelter actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar shelter {Id}.", shelter.Id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar shelter", "-1", null);
            }

        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<Shelter>> UpdateShelterName(int id, string name)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar nombre.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                shelter.Name = name;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nombre del shelter {Id} actualizado correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Nombre actualizado correctamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar nombre del shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar nombre", "-1", null);
            }
        }

        public async Task<GlobalResponse<Shelter>> UpdateShelterAddress(int id, string address)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar nombre.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                shelter.Address = address;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nombre del shelter {Id} actualizado correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Nombre actualizado correctamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar nombre del shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar nombre", "-1", null);
            }
        }

        public async Task<GlobalResponse<Shelter>> UpdateShelterCoordinates(int id, decimal latitude, decimal longitude)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar nombre.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                shelter.Latitude = latitude;
                shelter.Longitude = longitude;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nombre del shelter {Id} actualizado correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Nombre actualizado correctamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar nombre del shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar nombre", "-1", null);
            }
        }

        public async Task<GlobalResponse<Shelter>> UpdateShelterDescription(int id, string description)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);
                if (shelter == null)
                {
                    _logger.LogWarning("Shelter {Id} no encontrado para actualizar nombre.", id);
                    return GlobalResponse<Shelter>.Fault("Shelter no encontrado", "404", null);
                }

                shelter.Description = description;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Nombre del shelter {Id} actualizado correctamente.", id);
                return GlobalResponse<Shelter>.Success(shelter, 1, "Nombre actualizado correctamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar nombre del shelter {Id}.", id);
                return GlobalResponse<Shelter>.Fault("Error al actualizar nombre", "-1", null);
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
                return GlobalResponse<Shelter>.Success(null, 0, "Shelter eliminado exitosamente", "200");
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