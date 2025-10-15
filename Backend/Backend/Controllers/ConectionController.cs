using Backend.Infraestructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    /// <summary>
    /// Controlador para verificar la conectividad con la base de datos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")] 
    public class ConnectionController : ControllerBase
    {
        private readonly NeonTechDbContext _context;

        public ConnectionController(NeonTechDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si la aplicación puede conectarse a la base de datos configurada.
        /// </summary>
        /// <returns>Mensaje indicando si la conexión fue exitosa o fallida.</returns>
        [HttpGet("check-db")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            { 
                var canConnect = await _context.Database.CanConnectAsync();
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

        [HttpGet("tables")]
        public async Task<IActionResult> GetTables()
        {
            var tablas = await _context.Database
                .SqlQuery<string>($@"
                    SELECT table_name
                    FROM information_schema.tables
                    WHERE table_schema = 'public';
                ")
                .ToListAsync();

            return Ok(tablas);
        }

        [HttpGet("tables/{tableName}/columns")]
        public async Task<IActionResult> GetTableColumns(string tableName)
        {
            var columnas = await _context.Database
                .SqlQuery<ColumnInfo>($@"
                    SELECT 
                        column_name AS ""Name"",
                        data_type AS ""Type"",
                        is_nullable AS ""IsNullable""
                    FROM information_schema.columns
                    WHERE table_schema = 'public' AND table_name = {tableName};
                ")
                .ToListAsync();

            return Ok(columnas);
        }

        public class ColumnInfo
        {
            public string Name { get; set; } = "";
            public string Type { get; set; } = "";
            public string IsNullable { get; set; } = "";
        }
    }
}
