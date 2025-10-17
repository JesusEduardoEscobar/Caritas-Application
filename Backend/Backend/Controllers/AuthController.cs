using Backend.Infraestructure.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Interfaces;

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
            catch (Exception)
            {
                var errorResponse = GlobalResponse<string>.Fault("Error interno del servidor", "-1", null);
                return StatusCode(500, errorResponse);
            }
        }


    }
}