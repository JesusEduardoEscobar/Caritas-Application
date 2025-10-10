using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class SheltersManager : IShelters
    {
        private readonly NeonTechDbContext _context;

        public SheltersManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public async Task<GlobalResponse<IEnumerable<dynamic>>> GetShelters()
        {
            try
            {
                var shelters = await _context.Shelters.ToListAsync();

                if (shelters == null || !shelters.Any())
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Shelter no encontrado", "404", null);
                }

                return GlobalResponse<IEnumerable<dynamic>>.Success(shelters, shelters.Count, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault("Error al procesar Shelters", "-1", null);
            }
        }

        public async Task<GlobalResponse<dynamic>> GetShelter(int id)
        {
            try
            {
                var shelter = await _context.Shelters.FindAsync(id);

                if (shelter == null)
                {
                    return GlobalResponse<dynamic>.Fault("Shelter no encontrado", "404", null);
                }

                return GlobalResponse<dynamic>.Success(shelter, 1, "Obtención de Shelters exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault("Error al procesar Shelters", "-1", null);
            }
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateShelter(Shelter shelter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateShelter(Shelter shelter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateShelterAddress(int id, string address)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterCoordinates(int id, decimal latitude, decimal longitude)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterDescription(int id, string description)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterName(int id, string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteShelter(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}