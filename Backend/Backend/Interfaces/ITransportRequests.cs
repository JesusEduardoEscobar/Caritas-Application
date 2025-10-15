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
    public interface ITransportRequests
    {
        // Get
        Task<GlobalResponse<IEnumerable<TransportRequest>>> GetTransportRequests();
        Task<GlobalResponse<TransportRequest>> GetTransportRequest(int transportRequestId);

        // Post
        Task<GlobalResponse<TransportRequest>> CreateShelterService(TransportRequestCreateDto transportRequestId);

        // Patch
        Task<GlobalResponse<TransportRequest>> PatchShelterService(TransportRequestPatchDto transportRequestDto);

        // Delete
        Task<GlobalResponse<TransportRequest>> DeleteShelterService(int transportRequestId);
    }
}
