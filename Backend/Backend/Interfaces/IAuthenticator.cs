using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Infraestructure.Implementations;

namespace Backend.Interfaces
{
    public interface IAuthenticator
    {
        Task<GlobalResponse<dynamic>> Login(string email, string password);
        // REGISTRO DE LOS USUARIOS
        Task<GlobalResponse<dynamic>> RegisterLite(string nombre, string password, string numero);
        Task<GlobalResponse<dynamic>> RegisterUser(string nombre, string email, string password, string numero, string nivelEconomico, bool verifacion);
        Task<GlobalResponse<dynamic>> RegisterAdmin(string name, string email, string password, string emailAdmin, string passwordAdmin);
        // CHECAR QUE LOS USUARIOS SI ESTEN VERIFICADOS
        Task<GlobalResponse<dynamic>> VerifyUser(int id, bool verificacion);

    }
}
