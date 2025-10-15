using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Interfaces
{
    public interface IServiceReservations
    {
        // Get
        Task<GlobalResponse<IEnumerable<ServiceReservation>>> GetServiceReservations();
        Task<GlobalResponse<ServiceReservation>> GetServiceReservation(int serviceReservationId);

        // Post
        Task<GlobalResponse<ServiceReservation>> CreateShelterService(ServiceReservationCreateDto serviceReservationDto);

        // Delete
        Task<GlobalResponse<ServiceReservation>> DeleteShelterService(int serviceReservationId);
    }
}
