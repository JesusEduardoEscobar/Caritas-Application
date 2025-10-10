using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class CarsManager : ICars
    {
        private readonly NeonTechDbContext _context;

        public CarsManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetCars()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableCars()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetCarsByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }
        
        public Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableCarsByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateCar(Car car)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateCar(Car car)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateCarShelter(int id, int shelterId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteCar(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}