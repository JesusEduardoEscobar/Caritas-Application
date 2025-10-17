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
        Task<GlobalResponse<IEnumerable<Reservation>>> GetReservations(int? shelterId = null, int? userId = null, ReservationStatus? status = null);
        Task<GlobalResponse<Reservation>> GetReservation(int id);

        // Post
        Task<GlobalResponse<Reservation>> CreateReservation(ReservationCreateDto reservationDto);

        // Put
        Task<GlobalResponse<Reservation>> UpdateReservation(ReservationUpdateDto reservationDto);

        // Patch
        Task<GlobalResponse<Reservation>> UpdateReservationStatus(ReservationUpdateStatusDto reservationDto);
        Task<GlobalResponse<Reservation>> UpdateReservationPeriod(ReservationUpdatePeriodDto reservationDto);

        // Delete
        Task<GlobalResponse<Reservation>> DeleteReservation(int id);
    }
}
