using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Objects.Model;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly NeonTechDbContext _context;
        private readonly IUsers _users;

        public UsersController(NeonTechDbContext context, IUsers users)
        {
            _context = context;
            _users = users;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "401", null));
                }

                var response = await _users.LoginUser(request.Email, request.Password);

                if (response == null || response.Data == null || !response.Data.Any())
                {
                    return BadRequest(GlobalResponse<string>.Fault("Credenciales inválidas", "401", null));
                }

                return Ok(response);
            }
            catch (Exception ex)
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
                var response = await _users.RegisterLite(request.Nombre, request.Password, request.Numero);
                if(response == null || response.Data == null || !response.Data.Any())
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
                if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Numero) || string.IsNullOrWhiteSpace(request.NivelEconomico) || request.Verificacion == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "400", null));
                }
                var response = await _users.RegisterUser(request.Nombre, request.Email, request.Password, request.Numero, request.NivelEconomico, request.Verificacion.Value);
                if (response == null || response.Data == null || !response.Data.Any())
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
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegistrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.EmailAdmin) || string.IsNullOrWhiteSpace(request.PasswordAdmin))
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos puede estar vacío", "400", null));
                }
                var response = await _users.RegisterAdmin(request.Email, request.Password, request.EmailAdmin, request.PasswordAdmin);
                if (response == null || response.Data == null || !response.Data.Any())
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
                var response = await _users.VerifyUser(request.Id, request.Verificacion);
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


    }
}