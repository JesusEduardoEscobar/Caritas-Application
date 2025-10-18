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
    public interface ITransportRequests
    {
        // Get
        Task<GlobalResponse<IEnumerable<TransportRequest>>> GetTransportRequests(int? userId = null, int? shelterId = null, DateTime? requestDate = null, ReservationStatus? status = null);
        Task<GlobalResponse<TransportRequest>> GetTransportRequest(int id);
        Task<GlobalResponse<TransportRequest>> GetTransportRequest(string qrData);

        // Post
        Task<GlobalResponse<TransportRequest>> CreateTransportRequest(TransportRequestCreateDto dto);

        // Patch
        Task<GlobalResponse<TransportRequest>> UpdateTransportRequest(TransportRequestPatchDto dto);
        Task<GlobalResponse<TransportRequest>> UpdateTransportRequestStatus(TransportRequestPatchStatusDto dto);

        // Delete
        Task<GlobalResponse<TransportRequest>> DeleteTransportRequest(int id);
    }
}
