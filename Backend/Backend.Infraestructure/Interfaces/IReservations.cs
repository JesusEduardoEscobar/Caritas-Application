    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface IReservations
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetReservations();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByShelter(int shelterId);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByUser(int userID);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetReservationsByStatus(ReservationStatus status);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetReservations(int shelterId, ReservationStatus status);
            Task<GlobalResponse<dynamic>> GetReservation(int id);

            // Post
            Task<GlobalResponse<dynamic>> CreateReservation(Reservation reservation);

            // Put
            Task<GlobalResponse<dynamic>> UpdateReservation(Reservation reservation);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateReservationStatus(int id, ReservationStatus status);
            Task<GlobalResponse<dynamic>> UpdateReservationPeriod(int id, DateTime startDate, DateTime endDate);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteReservation(int id);
        }
    }
