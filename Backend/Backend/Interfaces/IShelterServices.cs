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
    public interface IShelterServices
    {
        // Get
        Task<GlobalResponse<IEnumerable<ShelterService>>> GetShelterServices();
        Task<GlobalResponse<IEnumerable<ShelterService>>> GetShelterServicesByShelter(int shelterId);
        Task<GlobalResponse<ShelterService>> GetShelterService(int shelterId, int serviceId);

        // Post
        Task<GlobalResponse<ShelterService>> CreateShelterService(ShelterServiceCreateDto dto);

        // Put
        Task<GlobalResponse<ShelterService>> UpdateShelterService(ShelterServicePutDto dto);

        // Delete
        Task<GlobalResponse<ShelterService>> DeleteShelterService(int shelterId, int serviceId);
    }
}
