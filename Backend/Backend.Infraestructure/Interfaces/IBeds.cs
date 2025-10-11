    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface IBeds
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetBeds();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableBeds();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetBedsByShelter(int shelterId);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableBedsByShelter(int shelterId);

            // Post
            Task<GlobalResponse<dynamic>> CreateBed(Bed bed);

            // Put
            Task<GlobalResponse<dynamic>> UpdateBed(Bed bed);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateBedNumber(int id, int bedNumber); // puede ser ???
            Task<GlobalResponse<dynamic>> UpdateBedAvailability(int id, bool isAvailable);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteBed(int id);
        }
    }
