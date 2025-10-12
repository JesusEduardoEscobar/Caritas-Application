using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ServicesManager : IServices
    {
        private readonly NeonTechDbContext _context;
        private readonly ILogger<SheltersManager> _logger;

        public ServicesManager(NeonTechDbContext context, ILogger<SheltersManager> logger)
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
                if (services == null) return GlobalResponse<IEnumerable<Service>>.Fault("Shelter no encontrado", "404", null);

                return GlobalResponse<IEnumerable<Service>>.Success(services, services.Count, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<Service>>.Fault("Error al procesar autenticación: " + ex.Message, "-1", null);
            }
        }

        public async Task<GlobalResponse<Service>> GetService(int id)
        {
            try
            {
                var service = await _context.Services.FindAsync(id);

                if (service == null) return GlobalResponse<Service>.Fault("Shelter no encontrado", "404", null);

                return GlobalResponse<Service>.Success(service, 1, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<Service>.Fault("Error al procesar autenticación: " + ex.Message, "-1", null);
            }
        }

        #endregion

        #region POST

        public Task<GlobalResponse<Service>> CreateService(Service service)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<Service>> UpdateService(Service service)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<Service>> UpdateServiceName(int id, string name)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<Service>> UpdateServiceDescription(int id, string description)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<Service>> UpdateServiceIconKey(int id, string iconKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<Service>> DeleteService(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}