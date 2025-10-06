using Backend.Implementations.Logic;
using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using System.Net;

namespace Backend.Controllers
{
    [Route("api")]
    [ApiController]
    /// <summary>
    /// Controlador para verificar la conectividad con la base de datos.
    /// </summary>
    public class ConectionController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ConectionController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        [HttpGet("check-db")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var response = await _connectionService.VerificarConexionAsync();

                if (response.Data == null || !response.Data.Any())
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = GlobalResponse<IEnumerable<dynamic>>.Fault(ex.Message, "-1", null);
                return StatusCode(500, errorResponse);
            }
        }
    }


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsers _userService;

        public AuthController(IUsers userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 200)]
        [ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 401)]
        [ProducesResponseType(typeof(GlobalResponse<IEnumerable<dynamic>>), 500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _userService.LoginUser(request.Email, request.Password);
            return StatusCode(int.Parse(response.Code), response);
        }
    }

}
