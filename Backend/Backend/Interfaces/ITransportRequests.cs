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
        Task<GlobalResponse<IEnumerable<TransportRequest>>> GetTransportRequests();
        Task<GlobalResponse<TransportRequest>> GetTransportRequest(int transportRequestId);

        // Post
        Task<GlobalResponse<TransportRequest>> CreateTransportRequest(TransportRequestCreateDto transportRequestDto);

        // Patch
        Task<GlobalResponse<TransportRequest>> PatchTransportRequest(TransportRequestPatchDto transportRequestDto);

        // Delete
        Task<GlobalResponse<TransportRequest>> DeleteTransportRequest(int transportRequestId);
    }
}
