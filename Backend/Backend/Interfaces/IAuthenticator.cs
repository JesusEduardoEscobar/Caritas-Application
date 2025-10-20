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
        Task<GlobalResponse<dynamic>> RegisterLite(string nombre, string email, string password, string numero, DateTime fechaDeNamicimiento);
        Task<GlobalResponse<dynamic>> RegisterUser(string email, string numero, int shelterId, string nivelEconomico, bool verificacion);
        Task<GlobalResponse<dynamic>> RegisterAdmin(string name, string email, string password, string emailAdmin, string passwordAdmin);
        // CHECAR QUE LOS USUARIOS SI ESTEN VERIFICADOS
        Task<GlobalResponse<dynamic>> VerifyUser(int id, bool verificacion);
        // ELIMINAR USUARIOS
        Task<GlobalResponse<dynamic>> DeleteUser(int id);
        //EDITAR USUARIOS
        Task<GlobalResponse<dynamic>> EditUser(int id, string? nombre, string? numero, int? shelterId, bool? verificado, string? nivelEconomico);

    }
}
