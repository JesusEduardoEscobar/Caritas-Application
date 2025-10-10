using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Backend.Implementations
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
                var user = await _context.Users
                    .Where(u => u.Email == email && u.Password == password)
                    .Select(u => new { u.Id, u.Email, u.Name })
                    .ToListAsync();

                if (user == null || !user.Any())
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Usuario no encontrado", "404", null);
                }

                return GlobalResponse<IEnumerable<dynamic>>.Success(user, user.Count, "Login exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault("Error al procesar login", "-1", null);
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
