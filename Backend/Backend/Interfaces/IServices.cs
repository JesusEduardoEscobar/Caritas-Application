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
    public interface IServices
    {
        // Get
        Task<GlobalResponse<IEnumerable<Service>>> GetServices();
        Task<GlobalResponse<Service>> GetService(int id);

        // Post
        Task<GlobalResponse<Service>> CreateService(ServiceCreateDto dto);

        // Put
        Task<GlobalResponse<Service>> UpdateService(ServicePutDto dto);

        // Delete
        Task<GlobalResponse<Service>> DeleteService(int id);
    }
}
