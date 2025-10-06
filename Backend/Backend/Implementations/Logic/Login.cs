using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infrastructure.Database;
using Backend.Infraestructure.Objects.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations.Logic
{
    public class Login : IUsers
    {
        private readonly NeonTechDbContext _context;

        public Login(NeonTechDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalResponse<IEnumerable<dynamic>>> LoginUser(string email, string password)
        {
            try
            {
                email = "roberto@gmail.com";
                password = "12345";
                if (password == null || email == null)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault(
                        message: "La contraseña o correo no pueden estar vacios.",
                        errorCode: "401",
                        data: new List<dynamic>()
                        );
                }
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault(
                        message: "El correro o la contraseña no no son correctos",
                        errorCode: "401",
                        data: new List<dynamic>()
                        );
                }

                var result = new List<dynamic>
                {
                    new {
                        user.Id,
                        user.Name,
                        user.Email,
                        user.Age,
                        user.EconomicLevel,
                        user.verificate,
                        user.shelter
                    }
                };
                return GlobalResponse<IEnumerable<dynamic>>.Success(
                   data: result,
                   rows: result.Count(),
                   message: "Login exitoso",
                   code: "200"
               );
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault(
                    message: $"Error interno: {ex.Message}",
                    errorCode: "500",
                    data: new List<dynamic>()
                );
            }
        }

        Task<GlobalResponse<IEnumerable<dynamic>>> IUsers.FilterByShelter()
        {
            throw new NotImplementedException();
        }

        Task<GlobalResponse<IEnumerable<dynamic>>> IUsers.GetUsers()
        {
            throw new NotImplementedException();
        }

        Task<GlobalResponse<IEnumerable<dynamic>>> IUsers.GetUsersByOne()
        {
            throw new NotImplementedException();
        }

        Task<GlobalResponse<IEnumerable<dynamic>>> IUsers.LoginAdmin(string emai, string password)
        {
            throw new NotImplementedException();
        }

        Task<GlobalResponse<IEnumerable<dynamic>>> IUsers.RegisterUser()
        {
            throw new NotImplementedException();
        }
    }
}
