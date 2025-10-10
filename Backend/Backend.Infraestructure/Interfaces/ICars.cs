    using Backend.Infraestructure.Implementations;
    using Backend.Infraestructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Backend.Infraestructure.Interfaces
    {
        public interface ICars
        {
            // Get
            Task<GlobalResponse<IEnumerable<dynamic>>> GetCars();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableCars();
            Task<GlobalResponse<IEnumerable<dynamic>>> GetCarsByShelter(int shelterId);
            Task<GlobalResponse<IEnumerable<dynamic>>> GetAvailableCarsByShelter(int shelterId);

            // Post
            Task<GlobalResponse<dynamic>> CreateCar(Car car);

            // Put
            Task<GlobalResponse<dynamic>> UpdateCar(Car car);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateCarShelter(int id, int shelterId);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteCar(int id);
        }
    }
