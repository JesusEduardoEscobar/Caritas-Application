using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class TransportRequestsManager : ITransportRequests
    {
        private readonly NeonTechDbContext _context;

        public TransportRequestsManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET
        
        public Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequests()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByCar(int carId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByUser(int userID)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByStatus(ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequests(int carId, ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> GetTransportRequest(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateTransportRequest(TransportRequest transportRequest)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateTransportRequest(TransportRequest transportRequest)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateTransportRequestStatus(int id, ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateTransportRequestDate(int id, DateTime requestDate)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateTransportRequestPickupLocation(int id, string location)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateTransportRequestDropoffLocation(int id, string location)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<dynamic>> DeleteReservation(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}