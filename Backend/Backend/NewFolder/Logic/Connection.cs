using Microsoft.AspNetCore.Mvc;
using System;
using Backend.Infrastructure.Database;

namespace Backend.NewFolder.Logic
{
    public class Connection
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ConnectionController : ControllerBase
        {
            [HttpGet("check-db")]
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
        }
    }
}
