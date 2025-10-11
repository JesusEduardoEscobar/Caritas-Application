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

        public ServicesManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetServices()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> GetService(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateService(Service service)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateService(Service service)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateServiceName(int id, string name)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateServiceDescription(int id, string description)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateServiceIconKey(int id, string iconKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteService(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}