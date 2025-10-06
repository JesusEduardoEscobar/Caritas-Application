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
}
