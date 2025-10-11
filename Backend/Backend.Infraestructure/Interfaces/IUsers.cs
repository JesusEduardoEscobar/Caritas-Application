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

        // Register user
        Task<GlobalResponse<IEnumerable<dynamic>>> RegisterLite(string nombre, string password, string numero);
        Task<GlobalResponse<IEnumerable<dynamic>>> RegisterUser(string nombre, string email, string password, string numero, string nivelEconomico, bool verifacion);
        Task<GlobalResponse<IEnumerable<dynamic>>> RegisterAdmin(string email, string password, string emailAdmin, string passwordAdmin);
                
        // Verificacion del usuario
        Task<GlobalResponse<IEnumerable<dynamic>>> VerifyUser(int id, bool verificacion);

        // Show users
        //Task<GlobalResponse<IEnumerable<dynamic>>> GetUsers();
        //Task<GlobalResponse<IEnumerable<dynamic>>> GetUsersByOne();
        //Task<GlobalResponse<IEnumerable<dynamic>>> FilterByShelter();
    }
}
