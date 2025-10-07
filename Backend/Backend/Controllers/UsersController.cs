using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
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

        public UsersController(NeonTechDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(string email, string password)
        {
            try
            {
                if (email == null || password == null)
                {
                    return BadRequest(GlobalResponse<string>.Fault("Ninguno de los campos pueden estar vacios", "401", null));
                }
                var response = await _users.LoginUser(email, password);

                if (response == null || response.Data == null)
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


    }
}