using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class BedsManager : IBeds
    {
        private readonly NeonTechDbContext _context;

        public BedsManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetBeds()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableBeds()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetBedsByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableBedsByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateBed(Bed bed)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateBed(Bed bed)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateBedNumber(int id, int bedNumber)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateBedAvailability(int id, bool isAvailable)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteBed(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}