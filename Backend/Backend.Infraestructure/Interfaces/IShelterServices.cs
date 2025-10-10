    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface IShelterServices
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetShelterServices();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetShelterServicesByShelter(int shelterId);
            Task<GlobalResponse<dynamic>> GetShelterService(int shelterId, int serviceId);

            // Post
            Task<GlobalResponse<dynamic>> CreateShelterService(ShelterService shelterService);

            // Put
            Task<GlobalResponse<dynamic>> UpdateShelterService(ShelterService shelterService);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateShelterServicePrice(int shelterId, int serviceId, decimal price);
            Task<GlobalResponse<dynamic>> UpdateShelterServiceAvailability(int shelterId, int serviceId, bool isAvailable);
            Task<GlobalResponse<dynamic>> UpdateShelterServiceDescription(int shelterId, int serviceId, string? description);
            Task<GlobalResponse<dynamic>> UpdateShelterServiceCapacity(int shelterId, int serviceId, int capacity);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteShelterService(int shelterId, int serviceId);
        }
    }
