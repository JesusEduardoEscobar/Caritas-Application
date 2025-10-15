using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
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

                if (cars == null || cars.Count == 0)
                {
                    _logger.LogWarning("No se encontraron carros en la base de datos.");
                    return GlobalResponse<IEnumerable<Car>>.Fault("Carros no encontradas", "404", null);
                }

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

        public async Task<GlobalResponse<Car>> CreateCar(CarCreateDto carDto)
        {
            try
            {
                if (carDto == null)
                {
                    _logger.LogWarning("Intento de crear carro con datos nulos.");
                    return GlobalResponse<Car>.Fault("Datos inválidos", "400", null);
                }

                bool shelterExists = await _context.Shelters
                    .AnyAsync(s => s.Id == carDto.ShelterId);
                if (!shelterExists)
                {
                    _logger.LogWarning("El ShelterId {ShelterId} no existe.", carDto.ShelterId);
                    return GlobalResponse<Car>.Fault($"El refugio con ID {carDto.ShelterId} no existe.", "404", null);
                }

                var car = new Car
                {
                    ShelterId = carDto.ShelterId,
                    Plate = carDto.Plate,
                    Model = carDto.Model,
                    Capacity = carDto.Capacity
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

        public async Task<GlobalResponse<Car>> UpdateCar(CarUpdateDto carDto)
        {
            try
            {
                if (carDto.ShelterId != null)
                {
                    bool shelterExists = await _context.Shelters
                        .AnyAsync(s => s.Id == carDto.ShelterId);
                    if (!shelterExists)
                    {
                        _logger.LogWarning("El ShelterId {ShelterId} no existe.", carDto.ShelterId);
                        return GlobalResponse<Car>.Fault($"El refugio con ID {carDto.ShelterId} no existe.", "404", null);
                    }
                }

                var existing = await _context.Cars.FindAsync(carDto.Id);
                if (existing == null)
                {
                    _logger.LogWarning("Carro {Id} no encontrada para actualizar.", carDto.Id);
                    return GlobalResponse<Car>.Fault("Carro no encontrada", "404", null);
                }

                if (carDto.ShelterId.HasValue) existing.ShelterId = carDto.ShelterId.Value;
                if (!string.IsNullOrEmpty(carDto.Plate)) existing.Plate = carDto.Plate;
                if (!string.IsNullOrEmpty(carDto.Model)) existing.Model = carDto.Model;
                if (carDto.Capacity.HasValue) existing.Capacity = carDto.Capacity.Value;

                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Carro {Id} actualizada correctamente.", carDto.Id);
                return GlobalResponse<Car>.Success(existing, 1, "Carro actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar carro {Id}.", carDto.Id);
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