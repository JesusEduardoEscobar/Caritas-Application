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
    public interface IServices
    {
        // Get
        Task<GlobalResponse<IEnumerable<Service>>> GetServices();
        Task<GlobalResponse<Service>> GetService(int id);

        // Post
        Task<GlobalResponse<Service>> CreateService(ServiceCreateDto serviceDto);

        // Put
        Task<GlobalResponse<Service>> UpdateService(ServiceUpdateDto serviceDto);

        // Delete
        Task<GlobalResponse<Service>> DeleteService(int id);
    }
}
