using Backend.Dtos;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;

namespace Backend.Interfaces
{
    public interface IServiceReservations
    {
        // Get
        Task<GlobalResponse<IEnumerable<ServiceReservation>>> GetServiceReservations(int? userId = null, int ? shelterId = null, int? serviceId = null, bool? isActive = null);
        Task<GlobalResponse<ServiceReservation>> GetServiceReservation(int id);
        Task<GlobalResponse<ServiceReservation>> GetServiceReservation(string qrData);


        // Post
        Task<GlobalResponse<ServiceReservation>> CreateServiceReservation(ServiceReservationCreateDto dto);

        // Patch
        Task<GlobalResponse<ServiceReservation>> UpdateServiceReservationIsActive(ServiceReservationPatchIsActiveDto dto);

        // Delete
        Task<GlobalResponse<ServiceReservation>> DeleteServiceReservation(int id);

        // Validar QR en base de datos
        Task<GlobalResponse<dynamic>> ValidateQr(string qrData);
    }
}
