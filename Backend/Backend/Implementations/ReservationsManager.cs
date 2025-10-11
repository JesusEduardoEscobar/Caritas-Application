using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class ReservationsManager : IReservations
    {
        private readonly NeonTechDbContext _context;

        public ReservationsManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetReservations()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByShelter(int shelterId)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByUser(int userID)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByStatus(ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<IEnumerable<dynamic>>> GetReservations(int shelterId, ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> GetReservation(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<dynamic>> CreateReservation(Reservation reservation)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region PUT

        public Task<GlobalResponse<dynamic>> UpdateReservation(Reservation reservation)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<dynamic>> UpdateReservationStatus(int id, ReservationStatus status)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<dynamic>> UpdateReservationPeriod(int id, DateTime startDate, DateTime endDate)
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