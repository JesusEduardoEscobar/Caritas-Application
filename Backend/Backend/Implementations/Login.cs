using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
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
        public async Task<GlobalResponse<dynamic>> LoginUser(string email, string password)
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
                    return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
                }

                var token = JwtHelper.GenerateToken(userEntity, _config);

                var result = new
                {
                    token,
                    user = new
                    {
                        userEntity.Id,
                        userEntity.Name,
                        userEntity.Email,
                        userEntity.Password,
                        userEntity.Age,
                        userEntity.Phone,
                        userEntity.EconomicLevel,
                        userEntity.Verified,
                        userEntity.ShelterId,
                        Role = userEntity.Role.ToString()
                    }
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Login exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al procesar login: {ex.Message}", "-1", null);
            }
        }
        public async Task<GlobalResponse<dynamic>> LoginAdmins(string email, string password)
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
                    return GlobalResponse<dynamic>.Fault("Credenciales inválidas", "401", null);
                }

                if (userEntity.Role != UserRole.admin)
                {
                    return GlobalResponse<dynamic>.Fault("Acceso restringido solo para administradores", "403", null);
                }

                var token = JwtHelper.GenerateToken(userEntity, _config);

                var result = new
                {
                    token,
                    user = new
                    {
                        userEntity.Id,
                        userEntity.Name,
                        userEntity.Email,
                        userEntity.Password,
                        userEntity.Age,
                        userEntity.Phone,
                        userEntity.EconomicLevel,
                        userEntity.Verified,
                        userEntity.ShelterId,
                        Role = userEntity.Role.ToString()
                    }
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Login exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al procesar login: {ex.Message}", "-1", null);
            }
        }



    // Registro de los usuarios
    public async Task<GlobalResponse<IEnumerable<dynamic>>> RegisterLite(string nombre, string password, string numero)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(numero))
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Todos los campos son obligatorios", "400", new List<dynamic>());
                }

                var existe = await _context.Users.AnyAsync(u => u.Name == nombre || u.Phone == numero);
                if (existe)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("El nombre o número ya están en uso", "409", new List<dynamic>());
                }

                if (!int.TryParse(numero, out int shelterId))
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Número inválido", "400", new List<dynamic>());
                }

                var nuevoUsuario = new User
                {
                    Name = nombre,
                    Password = password,
                    ShelterId = shelterId,
                    Verified = false,
                    Role = UserRole.user
                };

                _context.Users.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                var result = new List<dynamic> {
                    new {
                        nuevoUsuario.Id,
                        nuevoUsuario.Name,
                        nuevoUsuario.Phone
                    }
                };

                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al registrar usuario: {ex.Message}", "-1", new List<dynamic>());
            }
        }

        public async Task<GlobalResponse<IEnumerable<dynamic>>> RegisterUser(string nombre, string email, string password, string numero, string nivelEconomico, bool verificacion)
        {
            try
            {
                if (!int.TryParse(numero, out int shelterId))
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Número inválido", "400", new List<dynamic>());
                }

                var usuario = await _context.Users.FirstOrDefaultAsync(u => u.ShelterId == shelterId);


                if (string.IsNullOrWhiteSpace(usuario.Name)) usuario.Name = nombre;
                if (string.IsNullOrWhiteSpace(usuario.Email)) usuario.Email = email;
                if (string.IsNullOrWhiteSpace(usuario.Password)) usuario.Password = password;

                if (Enum.TryParse<EconomicLevel>(nivelEconomico, true, out var nivel))
                {
                    usuario.EconomicLevel = nivel;
                }

                usuario.Verified = verificacion;

                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();

                var result = new List<dynamic>
                {
                    new {
                        usuario.Id,
                        usuario.Name,
                        usuario.Email,
                        usuario.ShelterId,
                        usuario.EconomicLevel,
                        usuario.Verified
                    }
                };

                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al registrar usuario: {ex.Message}", "-1", new List<dynamic>());
            }
        }


        public async Task<GlobalResponse<IEnumerable<dynamic>>> RegisterAdmin(string name, string email, string password, string emailAdmin, string passwordAdmin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Todos los campos son obligatorios", "400", new List<dynamic>());
                }

                var existe = await _context.Users.AnyAsync(u => u.Email == email);
                if (existe)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("El nombre o número ya están en uso", "409", new List<dynamic>());
                }

                var verficado = await _context.Users.AnyAsync(u => u.Email == emailAdmin && u.Password == passwordAdmin);
                if (!verficado)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Credenciales de administrador inválidas", "403", new List<dynamic>());
                }
                else
                {
                    var nuevoUsuario = new User
                    {
                        Name = name,
                        Email = email,
                        Password = password,
                        Role = UserRole.admin,
                        Verified = true
                    };

                    _context.Users.Add(nuevoUsuario);
                    await _context.SaveChangesAsync();

                    var result = new List<dynamic> {
                    new {
                            nuevoUsuario.Id,
                            nuevoUsuario.Name,
                        }
                    };

                    return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Usuario registrado exitosamente", "201");
                }
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al registrar usuario: {ex.Message}", "-1", new List<dynamic>());
            }
        }

        public async Task<GlobalResponse<IEnumerable<dynamic>>> VerifyUser(int id, bool verificacion)
        {
            try
            {
                var usuario = await _context.Users.FindAsync(id);
                if (usuario == null)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Usuario no encontrado", "404", new List<dynamic>());
                }

                usuario.Verified = verificacion;
                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();

                var result = new List<dynamic>
                {
                    new {
                        usuario.Id,
                        usuario.Name,
                        usuario.Email,
                        usuario.ShelterId,
                        usuario.EconomicLevel,
                        usuario.Verified,
                        usuario.Role
                    }
                };

                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Verificación actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al actualizar verificación: {ex.Message}", "-1", new List<dynamic>());
            }
        }


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
