    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface IShelters
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetShelters();
            Task<GlobalResponse<dynamic>> GetShelter(int id);

            // Post
            Task<GlobalResponse<dynamic>> CreateShelter(Shelter shelter);

            // Put
            Task<GlobalResponse<dynamic>> UpdateShelter(Shelter shelter);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateShelterName(int id, string name);
            Task<GlobalResponse<dynamic>> UpdateShelterAddress(int id, string address);
            Task<GlobalResponse<dynamic>> UpdateShelterCoordinates(int id, decimal latitude, decimal longitude);
            Task<GlobalResponse<dynamic>> UpdateShelterDescription(int id, string description);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteShelter(int id);
        }
    }
