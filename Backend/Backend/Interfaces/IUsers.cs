using Backend.Infraestructure.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IUsers
    {  
        // Show users
        Task<GlobalResponse<IEnumerable<dynamic>>> GetUsers();
        Task<GlobalResponse<IEnumerable<dynamic>>> GetUserById(int id);
        Task<GlobalResponse<IEnumerable<dynamic>>> FilterByShelter(int shelterId);
    }
}
