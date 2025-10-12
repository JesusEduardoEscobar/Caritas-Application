using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;

namespace Backend.Implementations
{
    public class Authenticator : IAuthenticator
    {
        private readonly NeonTechDbContext _context;

        public Authenticator(NeonTechDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalResponse<dynamic>> Login(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Email == email && u.Password == password)
                    .FirstOrDefaultAsync();

                if (user == null) return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

                var token = GenerateJwtToken(user);

                var response = new
                {
                    Token = token,
                    User = user
                };

                return GlobalResponse<dynamic>.Success(response, 1, "Autenticación exitosa", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault("Error al procesar autenticación: " + ex.Message, "-1", null);
            }
        }

        private string GenerateJwtToken(User user)
        {
            string role = user.Role.ToString();
            role = char.ToUpper(role[0]) + role.Substring(1);

            var claims = new[] {new Claim(ClaimTypes.Role, role) };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("clave_secreta_muy_larga_123456789"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "tuapp",
                audience: "tuapp",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
