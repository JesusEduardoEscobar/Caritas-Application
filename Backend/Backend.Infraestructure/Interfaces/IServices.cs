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
            Task<GlobalResponse<IEnumerable<dynamic>>> GetServices();
            Task<GlobalResponse<dynamic>> GetService(int id);

            // Post
            Task<GlobalResponse<dynamic>> CreateService(Service service);

            // Put
            Task<GlobalResponse<dynamic>> UpdateService(Service service);

            // Patch
            Task<GlobalResponse<dynamic>> UpdateServiceName(int id, string name);
            Task<GlobalResponse<dynamic>> UpdateServiceDescription(int id, string description);
            Task<GlobalResponse<dynamic>> UpdateServiceIconKey(int id, string iconKey);

            // Delete
            Task<GlobalResponse<dynamic>> DeleteService(int id);
        }
    }
