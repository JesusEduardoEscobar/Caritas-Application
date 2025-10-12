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
            Task<GlobalResponse<IEnumerable<Shelter>>> GetShelters();
            Task<GlobalResponse<Shelter>> GetShelter(int id);

            // Post
            Task<GlobalResponse<Shelter>> CreateShelter(Shelter shelter);

            // Put
            Task<GlobalResponse<Shelter>> UpdateShelter(Shelter shelter);

            // Patch
            Task<GlobalResponse<Shelter>> UpdateShelterName(int id, string name);
            Task<GlobalResponse<Shelter>> UpdateShelterAddress(int id, string address);
            Task<GlobalResponse<Shelter>> UpdateShelterCoordinates(int id, decimal latitude, decimal longitude);
            Task<GlobalResponse<Shelter>> UpdateShelterDescription(int id, string description);

            // Delete
            Task<GlobalResponse<Shelter>> DeleteShelter(int id);
        }
    }
