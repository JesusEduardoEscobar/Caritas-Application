using Backend.Infraestructure.Implementations;
using Backend.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infraestructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using BCrypt.Net;

namespace Backend.Implementations
{
    public class Login : IUsers
    {
        private readonly NeonTechDbContext _context;
        private readonly IConfiguration _config;

        public Login(NeonTechDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Login de los usuarios
        //public async Task<GlobalResponse<dynamic>> LoginUser(string email, string password)
        //{
        //    try
        //    {
        //        var userEntity = await _context.Users
        //            .FirstOrDefaultAsync(u => u.Email == email);

        //        if (userEntity == null)
        //        {
        //            return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
        //        }

        //        if (!BCrypt.Net.BCrypt.Verify(password, userEntity.Password))
        //        {
        //            return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
        //        }

        //        var token = JwtHelper.GenerateToken(userEntity, _config);

        //        var result = new
        //        {
        //            token,
        //            user = new
        //            {
        //                userEntity.Id,
        //                userEntity.Name,
        //                userEntity.Email,
        //                userEntity.Password,
        //                userEntity.DateOfBirth,
        //                userEntity.Phone,
        //                userEntity.EconomicLevel,
        //                userEntity.Verified,
        //                userEntity.ShelterId,
        //                Role = userEntity.Role.ToString()
        //            }
        //        };

        //        return GlobalResponse<dynamic>.Success(result, 1, "Login exitoso", "200");
        //    }
        //    catch (Exception ex)
        //    {
        //        return GlobalResponse<dynamic>.Fault($"Error al procesar login: {ex.Message}", "-1", null);
        //    }
        //}
        //public async Task<GlobalResponse<dynamic>> LoginAdmins(string email, string password)
        //{
        //    try
        //    {
        //        var userEntity = await _context.Users
        //            .FirstOrDefaultAsync(u => u.Email == email);

        //        if (userEntity == null)
        //        {
        //            return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
        //        }

        //        if (!BCrypt.Net.BCrypt.Verify(password, userEntity.Password))
        //        {
        //            return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
        //        }

        //        if (userEntity.Role != UserRole.admin)
        //        {
        //            return GlobalResponse<dynamic>.Fault("Acceso restringido solo para administradores", "403", null);
        //        }

        //        var token = JwtHelper.GenerateToken(userEntity, _config);

        //        var result = new
        //        {
        //            token,
        //            user = new
        //            {
        //                userEntity.Id,
        //                userEntity.Name,
        //                userEntity.Email,
        //                userEntity.Password,
        //                userEntity.DateOfBirth,
        //                userEntity.Phone,
        //                userEntity.EconomicLevel,
        //                userEntity.Verified,
        //                userEntity.ShelterId,
        //                Role = userEntity.Role.ToString()
        //            }
        //        };

        //        return GlobalResponse<dynamic>.Success(result, 1, "Login exitoso", "200");
        //    }
        //    catch (Exception ex)
        //    {
        //        return GlobalResponse<dynamic>.Fault($"Error al procesar login: {ex.Message}", "-1", null);
        //    }
        // }



    // Registro de los usuarios
    


        // MOSTRAR USUARIOS
        public async Task<GlobalResponse<IEnumerable<dynamic>>> GetUsers()
        {
            try
            {
                var usuarios = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        u.Phone,
                        u.ShelterId,
                        u.DateOfBirth,
                        u.EconomicLevel,
                        u.Verified,
                        u.Role
                    })
                    .ToListAsync();
                if (usuarios == null || usuarios.Count == 0)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("No se encontraron usuarios", "404", new List<dynamic>());
                }
                return GlobalResponse<IEnumerable<dynamic>>.Success(usuarios, usuarios.Count, "Usuarios obtenidos exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al obtener usuarios: {ex.Message}", "-1", new List<dynamic>());
            }
        }

        public async Task<GlobalResponse<IEnumerable<dynamic>>> GetUserById(int id)
        {
            try
            {
                var usuario = await _context.Users
                    .Where(u => u.Id == id && u.Role == UserRole.user)
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        u.Phone,
                        u.ShelterId,
                        u.EconomicLevel,
                        u.Verified,
                        u.Role
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Usuario no encontrado", "404", new List<dynamic>());
                }

                return GlobalResponse<IEnumerable<dynamic>>.Success(new List<dynamic> { usuario }, 1, "Usuario obtenido exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al obtener usuario: {ex.Message}", "-1", new List<dynamic>());
            }
        }


        public async Task<GlobalResponse<IEnumerable<dynamic>>> FilterByShelter(int shelterId)
        {
            try
            {
                var usuarios = await _context.Users
                    .Where(u => u.ShelterId == shelterId)
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Email,
                        //u.Phone,
                        u.ShelterId,
                        u.EconomicLevel,
                        u.Verified,
                        u.Role
                    })
                    .ToListAsync();
                if (usuarios == null || usuarios.Count == 0)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("No se encontraron usuarios para el shelter especificado", "404", new List<dynamic>());
                }
                return GlobalResponse<IEnumerable<dynamic>>.Success(usuarios, usuarios.Count, "Usuarios obtenidos exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al obtener usuarios: {ex.Message}", "-1", new List<dynamic>());
            }
        }
    }
}
