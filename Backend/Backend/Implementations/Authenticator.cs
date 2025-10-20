using Backend.Infraestructure.Database;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using BCrypt.Net;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

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
                    token = token,
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

        public async Task<GlobalResponse<dynamic>> RegisterLite(string nombre, string email, string password, string numero, DateTime fechaDeNacimiento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return GlobalResponse<dynamic>.Fault("Todos los campos son obligatorios", "400", null);

                var existe = await _context.Users.AnyAsync(u => u.Email == email);
                if (existe)
                    return GlobalResponse<dynamic>.Fault("El correo ya está en uso", "409", null);
                if (fechaDeNacimiento > DateTime.Today)
                    return GlobalResponse<dynamic>.Fault("La fecha de nacimiento no puede ser futura", "400", null);
                if (!new EmailAddressAttribute().IsValid(email))
                    return GlobalResponse<dynamic>.Fault("Correo inválido", "400", null);
                if (!Regex.IsMatch(numero, @"^(\+52)?\d{10}$"))
                    return GlobalResponse<dynamic>.Fault("El número de teléfono debe tener 10 dígitos", "400", null);

                var nuevoUsuario = new User
                {
                    Name = nombre,
                    Email = email,
                    Phone = numero,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    EconomicLevel =  0,
                    DateOfBirth = fechaDeNacimiento,
                    Verified = false,
                    Role = UserRole.user
                };

                _context.Users.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                var result = new
                {
                    nuevoUsuario.Id,
                    nuevoUsuario.Name,
                    nuevoUsuario.Email,
                    nuevoUsuario.DateOfBirth
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al registrar usuario: {ex.Message}", "-1", null);
            }
        }

        // REGISTRARLOS USUARIOS POR PARTE DE LOS ADMINISTRADORES
        public async Task<GlobalResponse<dynamic>> RegisterUser(string email, string numero, int shelterId, string nivelEconomico, bool verificacion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return GlobalResponse<dynamic>.Fault("El correo es obligatorio", "400", null);

                var usuario = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (usuario == null)
                    return GlobalResponse<dynamic>.Fault("Usuario no encontrado con ese correo", "404", null);

                // Solo actualiza si están vacíos
                if (usuario.ShelterId == 0 || usuario.ShelterId == null) usuario.ShelterId = shelterId;

                usuario.Verified = verificacion;
                usuario.ShelterId = usuario.ShelterId == 0 ? shelterId : usuario.ShelterId;
                if (Enum.TryParse<EconomicLevel>(nivelEconomico, true, out var nivel))
                    usuario.EconomicLevel = nivel;
                if (usuario.DateOfBirth.HasValue && usuario.DateOfBirth.Value.Kind == DateTimeKind.Unspecified)
                {
                    usuario.DateOfBirth = DateTime.SpecifyKind(usuario.DateOfBirth.Value, DateTimeKind.Utc);
                }



                _context.Entry(usuario).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                var result = new
                {
                    usuario.Id,
                    usuario.Name,
                    usuario.Email,
                    usuario.DateOfBirth,
                    usuario.ShelterId,
                    usuario.EconomicLevel,
                    usuario.Verified
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Usuario actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al actualizar usuario: {ex.Message}", "-1", null);
            }
        }

        // CREAR USUARIOS
        public async Task<GlobalResponse<dynamic>> CreateUser(string name, string email, string password, string confirmPassword, string numero, DateTime fechaDeNacimiento, int shelterId, string? nivelEconomico, bool? verificacion)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
                    return GlobalResponse<dynamic>.Fault("Todos los campos son obligatorios", "400", null);

                if (password != confirmPassword)
                    return GlobalResponse<dynamic>.Fault("Las contraseñas no coinciden", "400", null);

                if (!new EmailAddressAttribute().IsValid(email))
                    return GlobalResponse<dynamic>.Fault("Correo inválido", "400", null);

                if (!Regex.IsMatch(numero, @"^(\+52)?\d{10}$"))
                    return GlobalResponse<dynamic>.Fault("El número de teléfono debe tener 10 dígitos", "400", null);

                if (fechaDeNacimiento > DateTime.Today)
                    return GlobalResponse<dynamic>.Fault("La fecha de nacimiento no puede ser futura", "400", null);

                var existe = await _context.Users.AnyAsync(u => u.Email == email);
                if (existe)
                    return GlobalResponse<dynamic>.Fault("El correo ya está en uso", "409", null);

                EconomicLevel nivel = 0;
                if (!string.IsNullOrWhiteSpace(nivelEconomico) && Enum.TryParse<EconomicLevel>(nivelEconomico, out var parsedNivel))
                    nivel = parsedNivel;

                var nuevoUsuario = new User
                {
                    Name = name,
                    Email = email,
                    Phone = numero,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    EconomicLevel = nivel,
                    DateOfBirth = fechaDeNacimiento,
                    Verified = verificacion ?? false,
                    Role = UserRole.user,
                    ShelterId = shelterId
                };

                _context.Users.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                var result = new
                {
                    nuevoUsuario.Id,
                    nuevoUsuario.Name,
                    nuevoUsuario.Email,
                    nuevoUsuario.DateOfBirth,
                    nuevoUsuario.ShelterId,
                    nuevoUsuario.Verified
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al registrar usuario: {ex.Message}", "-1", null);
            }
        }

        // REGISTAR ADMINSITRADORES, SOLO ADMINS PUEDEN CREAR ADMINSITRADORES
        public async Task<GlobalResponse<dynamic>> RegisterAdmin(string name, string email, string password, string emailAdmin, string passwordAdmin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
                    return GlobalResponse<dynamic>.Fault("Todos los campos son obligatorios", "400", null);

                var existe = await _context.Users.AnyAsync(u => u.Email == email);
                if (existe)
                    return GlobalResponse<dynamic>.Fault("El nombre o correo ya están en uso", "409", null);

                var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailAdmin);
                if (admin == null || !BCrypt.Net.BCrypt.Verify(passwordAdmin, admin.Password) || admin.Role != UserRole.admin)
                    return GlobalResponse<dynamic>.Fault("Credenciales de administrador inválidas", "403", null);

                var nuevoUsuario = new User
                {
                    Name = name,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = UserRole.admin,
                    Verified = true
                };

                _context.Users.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                var result = new
                {
                    nuevoUsuario.Id,
                    nuevoUsuario.Name
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Usuario registrado exitosamente", "201");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al registrar usuario: {ex.Message}", "-1", null);
            }
        }

        // VER SI EL USUARIO SI ESTA VERIFICADO
        public async Task<GlobalResponse<dynamic>> VerifyUser(int id, bool verificacion)
        {
            try
            {
                var usuario = await _context.Users.FindAsync(id);
                if (usuario == null)
                    return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

                usuario.Verified = verificacion;
                _context.Users.Update(usuario);
                await _context.SaveChangesAsync();

                var result = new
                {
                    usuario.Id,
                    usuario.Name,
                    usuario.Email,
                    usuario.ShelterId,
                    usuario.EconomicLevel,
                    usuario.Verified,
                    Role = usuario.Role.ToString()
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Verificación actualizada exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al actualizar verificación: {ex.Message}", "-1", null);
            }
        }

        // ELIMINAR USUARIOS
        public async Task<GlobalResponse<dynamic>> DeleteUser(int id)
        {
            try
            {
                var usuario = await _context.Users.FindAsync(id);
                if (usuario == null)
                    return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

                _context.Entry(usuario).State = EntityState.Deleted;
                await _context.SaveChangesAsync();

                return GlobalResponse<dynamic>.Success(null, 1, "Usuario eliminado exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault(
                        $"Error al eliminar usuario: {ex.Message} | StackTrace: {ex.StackTrace}",
                        "-1",
                        null
                    );
            }
        }


        // EDITAR USUARIOS
        public async Task<GlobalResponse<dynamic>> EditUser(int id, string? nombre, string? numero, int? shelterId, bool? verificado, string? nivelEconomico)
        {
            try
            {
                var usuario = await _context.Users.FindAsync(id);
                if (usuario == null)
                    return GlobalResponse<dynamic>.Fault("Usuario no encontrado", "404", null);

                if (!string.IsNullOrWhiteSpace(nombre))
                    usuario.Name = nombre;

                if (!string.IsNullOrWhiteSpace(numero))
                    usuario.Phone = numero;

                if (verificado.HasValue)
                    usuario.Verified = verificado.Value;

                if (!string.IsNullOrWhiteSpace(nivelEconomico))
                {
                    nivelEconomico = nivelEconomico.ToLower() switch
                    {
                        "bajo" => "Low",
                        "medio" => "Medium",
                        "alto" => "High",
                        _ => nivelEconomico
                    };

                    if (Enum.TryParse<EconomicLevel>(nivelEconomico, true, out var nivel))
                    {
                        usuario.EconomicLevel = nivel;
                    }
                    else
                    {
                        return GlobalResponse<dynamic>.Fault("Nivel económico inválido", "400", null);
                    }
                }


                if (shelterId.HasValue && (usuario.ShelterId == 0 || usuario.ShelterId == null))
                    usuario.ShelterId = shelterId;

                // ⬇️ ESTO ES CRÍTICO: Marca como modificado solo los campos que cambiaste
                _context.Entry(usuario).State = EntityState.Modified;

                // ⬇️ EVITA QUE EF INTENTE ACTUALIZAR DateOfBirth
                _context.Entry(usuario).Property(u => u.DateOfBirth).IsModified = false;

                // ⬇️ TAMBIÉN EVITA ACTUALIZAR CreatedAt si existe
                if (_context.Entry(usuario).Property("CreatedAt") != null)
                    _context.Entry(usuario).Property("CreatedAt").IsModified = false;

                await _context.SaveChangesAsync();

                var result = new
                {
                    usuario.Id,
                    usuario.Name,
                    usuario.Email,
                    usuario.ShelterId,
                    usuario.Verified,
                    usuario.EconomicLevel
                };

                return GlobalResponse<dynamic>.Success(result, 1, "Usuario actualizado exitosamente", "200");
            }
            catch (Exception ex)
            {
                return GlobalResponse<dynamic>.Fault($"Error al editar usuario: {ex.Message}", "-1", null);
            }
        }

    }

}