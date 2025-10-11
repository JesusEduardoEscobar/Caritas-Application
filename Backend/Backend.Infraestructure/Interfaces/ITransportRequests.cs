    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface ITransportRequests
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequests();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByCar(int carId);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByUser(int userID);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequestsByStatus(ReservationStatus status);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetTransportRequests(int carId, ReservationStatus status);
            Task<GlobalResponse<dynamic>> GetTransportRequest(int id);

            // Post
            Task<GlobalResponse<dynamic>> CreateTransportRequest(TransportRequest transportRequest);

            // Put
            Task<GlobalResponse<dynamic>> UpdateTransportRequest(TransportRequest transportRequest);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateTransportRequestStatus(int id, ReservationStatus status);
            Task<GlobalResponse<dynamic>> UpdateTransportRequestDate(int id, DateTime requestDate);
            Task<GlobalResponse<dynamic>> UpdateTransportRequestPickupLocation(int id, string location);
            Task<GlobalResponse<dynamic>> UpdateTransportRequestDropoffLocation(int id, string location);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteReservation(int id);
        }
    }
