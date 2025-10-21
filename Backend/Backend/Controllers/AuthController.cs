using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Models;
using Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticator _auth;
        
        public AuthController(IAuthenticator auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
        {
            try
            {
                if (request.Email == null || request.Password == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos pueden estar vacios", "401", null));
                }

                var response = await _auth.Login(request.Email, request.Password);

                if (response == null || response.Data == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault(response?.Message ?? "Credenciales inválidas", "401", null));
                }

                return Ok(response);
            }
            catch (Exception)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost("register-lite")]
        public async Task<IActionResult> RegisterLite([FromBody] UserRegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Numero))
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "400", null));
                }
                var response = await _auth.RegisterLite(request.Nombre, request.Email, request.Password, request.Numero, request.FechaDeNacimiento);
                if (response == null || response.Data == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al registrar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Numero) || string.IsNullOrWhiteSpace(request.NivelEconomico) || request.Verificacion == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "400", null));
                }
                var response = await _auth.RegisterUser(
                    request.Email,
                    request.Numero,
                    request.ShelterId,
                    request.NivelEconomico,
                    request.Verificacion ?? false
                );
                if (!response.Ok)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al registrar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] UserRegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Nombre) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Numero))
                {
                    return BadRequest(GlobalResponse<string>.Fault("Nombre, contraseña y número son obligatorios", "400", null));
                }

                var response = await _auth.CreateUser(
                    name: request.Nombre!,
                    email: request.Email,
                    password: request.Password!,
                    confirmPassword: request.Password!, // Asumes confirmación implícita
                    numero: request.Numero,
                    fechaDeNacimiento: request.FechaDeNacimiento,
                    shelterId: request.ShelterId,
                    nivelEconomico: request.NivelEconomico,
                    verificacion: request.Verificacion
                );

                if (response == null || response.Data == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al registrar usuario", "400", null));
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.EmailAdmin) || string.IsNullOrWhiteSpace(request.PasswordAdmin))
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "400", null));
                }
                var response = await _auth.RegisterAdmin(request.Nombre, request.Email, request.Password, request.EmailAdmin, request.PasswordAdmin);
                if (response == null || !response.Ok)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al registrar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("verify-user")]
        public async Task<IActionResult> VerifyUser([FromBody] VerifyUserRequest request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return BadRequest(GlobalResponse<string>.Fault("El ID del usuario debe ser mayor que cero", "400", null));
                }
                var response = await _auth.VerifyUser(request.Id, request.Verificacion);
                if (response == null || response.Data == null || !response.Data.Any())
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al verificar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequesst request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return BadRequest(GlobalResponse<string>.Fault("El ID del usuario debe ser mayor que cero", "400", null));
                }
                var response = await _auth.DeleteUser(request.Id);
                if (response == null || !response.Ok)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Error al eliminar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPatch("edit-user")]
        public async Task<IActionResult> EditUser([FromBody] EditUserRequest request)
        {
            try
            {
                if (request.Id <= 0)
                {
                    return BadRequest(GlobalResponse<string>.Fault("El ID del usuario debe ser mayor que cero", "400", null));
                }

                var response = await _auth.EditUser(
                    request.Id,
                    request.Nombre,
                    request.Numero,
                    request.ShelterId,  // ⬅️ AGREGAR ESTE PARÁMETRO
                    request.Verificado,
                    request.NivelEconomico
                );

                if (response == null || !response.Ok)
                {
                    return BadRequest(GlobalResponse<string>.Fault(response?.Message ?? "Error al editar usuario", "400", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault($"Error interno del servidor: {ex.Message}", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }
    }
}