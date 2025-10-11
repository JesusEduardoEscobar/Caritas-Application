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

        // Login de los usuarios
        public async Task<GlobalResponse<IEnumerable<dynamic>>> LoginUser(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Email == email && u.Password == password)
                    .Select(u => new { u.Id, u.Email, u.Name })
                    .FirstOrDefaultAsync();


                if (user == null)
                {
                    return GlobalResponse<IEnumerable<dynamic>>.Fault("Usuario no encontrado", "404", null);
                }
                var result = new List<dynamic> { user };
                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Login exitoso", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al procesar login: {ex.Message}", "-1", new List<dynamic>());
            }
        }

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

                var nuevoUsuario = new User
                {
                    Name = nombre,
                    Password = password,
                    Phone = numero
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
                var usuario = await _context.Users.FirstOrDefaultAsync(u => u.shelter == numero);

                if (string.IsNullOrWhiteSpace(usuario.Name)) usuario.Name = nombre;
                if (string.IsNullOrWhiteSpace(usuario.Email)) usuario.Email = email;
                if (string.IsNullOrWhiteSpace(usuario.Password)) usuario.Password = password;

                if (usuario.EconomicLevel == 0 && int.TryParse(nivelEconomico, out int nivel))
                {
                    usuario.EconomicLevel = nivel;
                }

                usuario.verificate = verificacion;

                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();

                var result = new List<dynamic>
                {
                    new {
                        usuario.Id,
                        usuario.Name,
                        usuario.Email,
                        usuario.shelter,
                        usuario.EconomicLevel,
                        usuario.verificate
                    }
                };

                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al registrar usuario: {ex.Message}", "-1", new List<dynamic>());
            }
        }


        public async Task<GlobalResponse<IEnumerable<dynamic>>> RegisterAdmin(string email, string password, string emailAdmin, string passwordAdmin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
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
                        Email = email,
                        Password = password,
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
                usuario.verificate = verificacion;
                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();
                var result = new List<dynamic>
                {
                    new {
                        usuario.Id,
                        usuario.Name,
                        usuario.Email,
                        usuario.shelter,
                        usuario.EconomicLevel,
                        usuario.verificate
                    }
                };
                return GlobalResponse<IEnumerable<dynamic>>.Success(result, 1, "Verificación actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault($"Error al actualizar verificación: {ex.Message}", "-1", new List<dynamic>());
            }
        }
    }
}
