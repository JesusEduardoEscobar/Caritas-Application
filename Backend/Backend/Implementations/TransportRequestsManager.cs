using Backend.Dtos;
using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
{
    public class TransportRequestsManager : ITransportRequests
    {
        private readonly NeonTechDbContext _context;

        public TransportRequestsManager(NeonTechDbContext context)
        {
            _context = context;
        }

        #region GET
        
        public Task<GlobalResponse<IEnumerable<TransportRequest>>> GetTransportRequests()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalResponse<TransportRequest>> GetTransportRequest(int transportRequestId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region POST

        public Task<GlobalResponse<TransportRequest>> CreateTransportRequest(TransportRequestCreateDto transportRequestDto)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PATCH

        public Task<GlobalResponse<TransportRequest>> PatchTransportRequest(TransportRequestPatchDto transportRequestDto)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region DELETE

        public Task<GlobalResponse<TransportRequest>> DeleteTransportRequest(int transportRequestId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}