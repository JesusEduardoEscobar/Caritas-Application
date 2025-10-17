using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Database;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

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
                var userEntity = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (userEntity == null)
                {
                    return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
                }

                if (!BCrypt.Net.BCrypt.Verify(password, userEntity.Password))
                {
                    return GlobalResponse<dynamic>.Fault("Formato de la constraseña incorecto", "401", null);
                }

                var token = GenerateJwtToken(userEntity);

                var response = new
                {
                    Token = token,
                    User = new
                    {
                        userEntity.Id,
                        userEntity.Name,
                        userEntity.Email,
                        userEntity.DateOfBirth,
                        userEntity.Phone,
                        userEntity.EconomicLevel,
                        userEntity.Verified,
                        userEntity.ShelterId,
                        Role = userEntity.Role.ToString()
                    }
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
