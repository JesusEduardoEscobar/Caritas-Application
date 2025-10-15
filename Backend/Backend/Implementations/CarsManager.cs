using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Database;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class CarsManager : ICars
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<CarsManager> _logger;

        public CarsManager(NeonTechDbContext context, ILogger<CarsManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<Car>>> GetCars(int? shelterId = null)
        {
            try
            {
                var query = _context.Cars.AsQueryable();

                if (shelterId.HasValue)
                    query = query.Where(b => b.ShelterId == shelterId.Value);

                var cars = await query.ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} carros correctamente.", cars.Count);
                return GlobalResponse<IEnumerable<Car>>.Success(cars, cars.Count, "Obtención de Carros exitoso", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener carros.");
                return GlobalResponse<IEnumerable<Car>>.Fault("Error al procesar Carros", "-1", null);
            }
        }

        public async Task<GlobalResponse<Car>> GetCar(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    _logger.LogWarning("Carro {Id} no encontrada.", id);
                    return GlobalResponse<Car>.Fault("Carro no encontrada", "404", null);
                }

                _logger.LogInformation("Carro {Id} obtenida correctamente.", id);
                return GlobalResponse<Car>.Success(car, 1, "Obtención de Carro exitosa", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Carro {Id}.", id);
                return GlobalResponse<Car>.Fault("Error al procesar Carros", "-1", null);
            }
        }

        #endregion

        #region POST

        public async Task<GlobalResponse<Car>> CreateCar(CarCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Intento de crear carro con datos nulos.");
                    return GlobalResponse<Car>.Fault("Datos inválidos", "400", null);
                }

                bool shelterExists = await _context.Shelters
                    .AnyAsync(s => s.Id == dto.ShelterId);
                if (!shelterExists)
                {
                    _logger.LogWarning("El ShelterId {ShelterId} no existe.", dto.ShelterId);
                    return GlobalResponse<Car>.Fault($"El refugio con ID {dto.ShelterId} no existe.", "404", null);
                }

                var car = new Car
                {
                    ShelterId = dto.ShelterId,
                    Plate = dto.Plate,
                    Model = dto.Model,
                    Capacity = dto.Capacity
                };

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Carro {Id} creada correctamente.", car.Id);
                return GlobalResponse<Car>.Success(car, 1, "Carro creada exitosamente", "201");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear carro.");
                return GlobalResponse<Car>.Fault("Error al crear carro", "-1", null);
            }
        }

        #endregion

        #region PUT

        public async Task<GlobalResponse<Car>> UpdateCar(CarPutDto dto)
        {
            try
            {
                bool shelterExists = await _context.Shelters
                    .AnyAsync(s => s.Id == dto.ShelterId);
                if (!shelterExists)
                {
                    _logger.LogWarning("El ShelterId {ShelterId} no existe.", dto.ShelterId);
                    return GlobalResponse<Car>.Fault($"El refugio con ID {dto.ShelterId} no existe.", "404", null);
                }

                var existing = await _context.Cars.FindAsync(dto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Carro {Id} no encontrada para actualizar.", dto.Id);
                    return GlobalResponse<Car>.Fault("Carro no encontrada", "404", null);
                }

                existing.ShelterId = dto.ShelterId;
                existing.Plate = dto.Plate;
                existing.Model = dto.Model;
                existing.Capacity = dto.Capacity;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Carro {Id} actualizada correctamente.", dto.Id);
                return GlobalResponse<Car>.Success(existing, 1, "Carro actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar carro {Id}.", dto.Id);
                return GlobalResponse<Car>.Fault("Error al actualizar carro", "-1", null);
            }
        }

        #endregion

        #region DELETE

        public async Task<GlobalResponse<Car>> DeleteCar(int id)
        {
            try
            {
                var car = await _context.Cars.FindAsync(id);
                if (car == null)
                {
                    _logger.LogWarning("Carro {Id} no encontrada para eliminar.", id);
                    return GlobalResponse<Car>.Fault("Carro no encontrada", "404", null);
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Carro {Id} eliminada correctamente.", id);
                return GlobalResponse<Car>.Success(car, 1, "Carro eliminada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar carro {Id}.", id);
                return GlobalResponse<Car>.Fault("Error al eliminar carro", "-1", null);
            }
        }

        #endregion

    }
}