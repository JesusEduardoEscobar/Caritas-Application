using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ShelterServicesManager : IShelterServices
    {
        private readonly NeonTechDbContext _context;

        public ShelterServicesManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetShelterServices()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetShelterServicesByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> GetShelterService(int shelterId, int serviceId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateShelterService(ShelterService shelterService)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateShelterService(ShelterService shelterService)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateShelterServicePrice(int shelterId, int serviceId, decimal price)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterServiceAvailability(int shelterId, int serviceId, bool isAvailable)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterServiceDescription(int shelterId, int serviceId, string? description)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateShelterServiceCapacity(int shelterId, int serviceId, int capacity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteShelterService(int shelterId, int serviceId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}