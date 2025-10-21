using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IReservations
    {
        // Get
        Task<GlobalResponse<IEnumerable<Reservation>>> GetReservations(int? shelterId = null, int? userId = null, ReservationStatus? status = null, DateTime? date = null);
        Task<GlobalResponse<Reservation>> GetReservation(int id);
        Task<GlobalResponse<Reservation>> GetReservation(string qrData);

        // Post
        Task<GlobalResponse<Reservation>> CreateReservation(ReservationCreateDto dto);

        // Patch
        Task<GlobalResponse<Reservation>> UpdateReservationStatus(ReservationPatchStatusDto dto);
        Task<GlobalResponse<Reservation>> UpdateReservationPeriod(ReservationPatchPeriodDto dto);

        // Delete
        Task<GlobalResponse<Reservation>> DeleteReservation(int id);
    }
}
