using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Database; // Asegúrate de que este namespace apunta a NeonTechDbContext

namespace Backend.Controllers
{
    /// <summary>
    /// Controlador para verificar la conectividad con la base de datos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionController : ControllerBase
    {
        /// <summary>
        /// Verifica si la aplicación puede conectarse a la base de datos configurada.
        /// </summary>
        /// <returns>Mensaje indicando si la conexión fue exitosa o fallida.</returns>
        [HttpGet("check-db")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CheckDatabase([FromServices] NeonTechDbContext context)
        {
            try
            { 
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok("✅ Conexión a la base de datos exitosa");
                }
                else
                {
                    return StatusCode(500, "❌ No se pudo conectar a la base de datos");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error de conexión: {ex.Message}");
            }
        }

        [HttpGet("show-tables")]
        public async Task<>
    }
}
