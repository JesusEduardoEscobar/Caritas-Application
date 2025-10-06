using Backend.Infraestructure.Implementations;
using Backend.Infraestructure.Interfaces;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Implementations.Logic
{
    public class Connection : IConnectionService
    {
        private readonly NeonTechDbContext _context;

        public Connection(NeonTechDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalResponse<IEnumerable<dynamic>>> VerificarConexionAsync()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                var data = new List<dynamic>
                {
                    new {
                        Estado = canConnect ? "Conectado" : "Desconectado",
                        Fecha = DateTime.UtcNow,
                        BaseDeDatos = _context.Database.GetDbConnection().Database,
                        Proveedor = _context.Database.ProviderName
                    }
                };

                return GlobalResponse<IEnumerable<dynamic>>.Success(
                    data,
                    rows: data.Count,
                    message: canConnect ? "✅ Conexión exitosa" : "❌ Falló la conexión",
                    code: canConnect ? "200" : "500"
                );
            }
            catch (Exception ex)
            {
                return GlobalResponse<IEnumerable<dynamic>>.Fault(
                    message: $"❌ Error: {ex.Message}",
                    errorCode: "500",
                    data: new List<dynamic>()
                );
            }
        }
    }
}
