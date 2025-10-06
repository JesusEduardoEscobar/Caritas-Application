using Backend.Infraestructure.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Interfaces
{
    public interface IUsers
    {
        // Log users
        Task<GlobalResponse<IEnumerable<dynamic>>> LoginUser(string email, string password);
        Task<GlobalResponse<IEnumerable<dynamic>>> LoginAdmin(string emai, string password);

        // Register user
        Task<GlobalResponse<IEnumerable<dynamic>>> RegisterUser();

        // Show users
        Task<GlobalResponse<IEnumerable<dynamic>>> GetUsers();
        Task<GlobalResponse<IEnumerable<dynamic>>> GetUsersByOne();
        Task<GlobalResponse<IEnumerable<dynamic>>> FilterByShelter();
    }
}
