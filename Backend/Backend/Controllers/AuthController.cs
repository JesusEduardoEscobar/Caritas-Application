using Backend.Implementations;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infraestructure.Models;
using Backend.Infrastructure.Database;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NeonTechDbContext _context;
        private readonly IAuthenticator _auth;
        
        public AuthController(NeonTechDbContext context, IAuthenticator auth)
        {
            _context = context;
            _auth = auth;
        }

        [HttpPost("login")]
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


    }
}