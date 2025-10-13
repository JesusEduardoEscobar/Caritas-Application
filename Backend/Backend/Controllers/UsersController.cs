using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infrastructure.Database;
using Backend.Implementations;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Backend.Infraestructure.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly NeonTechDbContext _context;
        private readonly IAuthenticator _auth;
        private readonly IUsers _users;

        public UsersController(NeonTechDbContext context, IAuthenticator auth, IUsers users)
        {
            _context = context;
            _auth = auth;
            _users = users;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        [HttpPost("loginUser")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                if (email == null || password == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos pueden estar vacios", "401", null));
                }
                var response = await _auth.Login(email, password);

                if (response == null || response.Data == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault(response?.Message ?? "Credenciales inválidas", "401", null));
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost("loginAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmins(string email, string password)
        {
            try
            {
                if (email == null || password == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos pueden estar vacios", "401", null));
                }
                var response = await _auth.Login(email, password);

                if (response == null || response.Data == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault(response?.Message ?? "Credenciales inválidas", "401", null));
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
                var response = await _users.RegisterAdmin(request.Nombre, request.Email, request.Password, request.EmailAdmin, request.PasswordAdmin);
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

        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _users.GetUsers();
                if (response == null || response.Data == null || !response.Data.Any())
                {
                    return NotFound(GlobalResponse<string>.Fault("No se encontraron usuarios", "404", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(GlobalResponse<string>.Fault("El ID del usuario debe ser mayor que cero", "400", null));
                }
                var response = await _users.GetUserById(id);
                if (response == null || response.Data == null || !response.Data.Any())
                {
                    return NotFound(GlobalResponse<string>.Fault("Usuario no encontrado", "404", null));
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("filter-by-shelter/{shelterId:int}")]
        public async Task<IActionResult> FilterByShelter(int shelterId)
        {
            try
            {
                if (shelterId <= 0)
                {
                    return BadRequest(GlobalResponse<string>.Fault("El ID del refugio debe ser mayor que cero", "400", null));
                }
                var response = await _users.FilterByShelter(shelterId);
                if (response == null || response.Data == null || !response.Data.Any())
                {
                    return NotFound(GlobalResponse<string>.Fault("No se encontraron usuarios para el refugio especificado", "404", null));
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