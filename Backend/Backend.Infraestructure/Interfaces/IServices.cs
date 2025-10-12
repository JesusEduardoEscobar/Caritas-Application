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
            Task<GlobalResponse<Service>> CreateService(Service service);

            // Put
            Task<GlobalResponse<Service>> UpdateService(Service service);

            // Patch
            Task<GlobalResponse<Service>> UpdateServiceName(int id, string name);
            Task<GlobalResponse<Service>> UpdateServiceDescription(int id, string description);
            Task<GlobalResponse<Service>> UpdateServiceIconKey(int id, string iconKey);

            // Delete
            Task<GlobalResponse<Service>> DeleteService(int id);
        }
    }
