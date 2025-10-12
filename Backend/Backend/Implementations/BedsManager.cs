using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class BedsManager : IBeds
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<BedsManager> _logger;

        public BedsManager(NeonTechDbContext context, ILogger<BedsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Bed>>> GetBeds(int? shelterId = null, bool? available = null)
        {
            try
            {
                var query = _context.Beds.AsQueryable();

                if (shelterId.HasValue)
                    query = query.Where(b => b.ShelterId == shelterId.Value);

                if (available.HasValue)
                    query = query.Where(b => b.IsAvailable == available.Value);

                var beds = await query.ToListAsync();

                if (beds == null || !beds.Any())
                {
                    _logger.LogWarning("No se encontraron camas en la base de datos.");
                    return GlobalResponse<IEnumerable<Bed>>.Fault("Camas no encontradas", "404", null);
                }

                _logger.LogInformation("Se obtuvieron {Count} camas correctamente.", beds.Count);
                return GlobalResponse<IEnumerable<Bed>>.Success(beds, beds.Count, "Obtención de Camas exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener camas.");
                return GlobalResponse<IEnumerable<Bed>>.Fault("Error al procesar Camas", "-1", null);
            }
        }

        public async Task<GlobalResponse<Bed>> GetBed(int id)
        {
            try
            {
                var bed = await _context.Beds.FindAsync(id);
                if (bed == null)
                {
                    _logger.LogWarning("Cama {Id} no encontrada.", id);
                    return GlobalResponse<Bed>.Fault("Cama no encontrada", "404", null);
                }

                _logger.LogInformation("Cama {Id} obtenida correctamente.", id);
                return GlobalResponse<Bed>.Success(bed, 1, "Obtención de Cama exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Cama {Id}.", id);
                return GlobalResponse<Bed>.Fault("Error al procesar Camas", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<Bed>> CreateBed(BedCreateDto bedDto)
        {
            try
            {
                if (bedDto == null)
                {
                    _logger.LogWarning("Intento de crear cama con datos nulos.");
                    return GlobalResponse<Bed>.Fault("Datos inválidos", "400", null);
                }

                var bed = new Bed
                {
                    ShelterId = bedDto.ShelterId,
                    BedNumber = bedDto.BedNumber,
                    IsAvailable = bedDto.IsAvailable
                };

                _context.Beds.Add(bed);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cama {Id} creada correctamente.", bed.Id);
                return GlobalResponse<Bed>.Success(bed, 1, "Cama creada exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cama.");
                return GlobalResponse<Bed>.Fault("Error al crear cama", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<Bed>> UpdateBed(BedUpdateDto bedDto)
        {
            try
            {
                var existing = await _context.Beds.FindAsync(bedDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Cama {Id} no encontrada para actualizar.", bedDto.Id);
                    return GlobalResponse<Bed>.Fault("Cama no encontrada", "404", null);
                }

                if (bedDto.ShelterId.HasValue) existing.ShelterId = bedDto.ShelterId.Value;
                if (bedDto.BedNumber.HasValue) existing.BedNumber = bedDto.BedNumber.Value;
                if (bedDto.IsAvailable.HasValue) existing.IsAvailable = bedDto.IsAvailable.Value;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cama {Id} actualizada correctamente.", bedDto.Id);
                return GlobalResponse<Bed>.Success(existing, 1, "Cama actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cama {Id}.", bedDto.Id);
                return GlobalResponse<Bed>.Fault("Error al actualizar cama", "-1", null);
            }
        }

        #endregion

        #region PATCH

        public async Task<GlobalResponse<Bed>> UpdateBedAvailability(BedUpdateAvailabilityDto bedDto)
        {
            try
            {
                var existing = await _context.Beds.FindAsync(bedDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Cama {Id} no encontrada para actualizar.", bedDto.Id);
                    return GlobalResponse<Bed>.Fault("Cama no encontrada", "404", null);
                }

                existing.IsAvailable = bedDto.IsAvailable;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cama {Id} actualizada correctamente.", bedDto.Id);
                return GlobalResponse<Bed>.Success(existing, 1, "Cama actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cama {Id}.", bedDto.Id);
                return GlobalResponse<Bed>.Fault("Error al actualizar cama", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<Bed>> DeleteBed(int id)
        {
            try
            {
                var bed = await _context.Beds.FindAsync(id);
                if (bed == null)
                {
                    _logger.LogWarning("Cama {Id} no encontrada para eliminar.", id);
                    return GlobalResponse<Bed>.Fault("Cama no encontrada", "404", null);
                }

                _context.Beds.Remove(bed);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cama {Id} eliminada correctamente.", id);
                return GlobalResponse<Bed>.Success(bed, 1, "Cama eliminada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cama {Id}.", id);
                return GlobalResponse<Bed>.Fault("Error al eliminar cama", "-1", null);
            }
        }

        #endregion

    }
}