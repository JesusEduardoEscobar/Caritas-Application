using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Backend.Infraestructure.Objects.Models.Tablas;

namespace Backend.Infraestructure.Objects.Models
{
    public class Tablas
    {
        public interface IMetadataService
        {
            List<TablaInfo> ObtenerTablasYColumnas();
        }

        public class MetadataService : IMetadataService
        {
            private readonly NeonTechDbContext _context;

            public MetadataService(NeonTechDbContext context)
            {
                _context = context;
            }

            public List<TablaInfo> ObtenerTablasYColumnas()
            {
                var metadata = _context.Model.GetEntityTypes();
                var resultado = new List<TablaInfo>();

                foreach (var entity in metadata)
                {
                    var tabla = new TablaInfo
                    {
                        NombreTabla = entity.GetTableName(),
                        Columnas = entity.GetProperties()
                            .Select(p => new ColumnaInfo
                            {
                                NombreColumna = p.Name,
                                TipoDato = p.ClrType.Name
                            }).ToList()
                    };
                    resultado.Add(tabla);
                }

                return resultado;
            }
        }

        public class TablaInfo
        {
            public string NombreTabla { get; set; }
            public List<ColumnaInfo> Columnas { get; set; }
        }

        public class ColumnaInfo
        {
            public string NombreColumna { get; set; }
            public string TipoDato { get; set; }
        }

    }
    public class MetadataRawService
    {
        private readonly NeonTechDbContext _context;

        public MetadataRawService(NeonTechDbContext context)
        {
            _context = context;
        }

        public async Task<List<TablaInfo>> ObtenerTodasLasTablasYColumnasAsync()
        {
            var query = @"
            SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE
            FROM INFORMATION_SCHEMA.COLUMNS
            ORDER BY TABLE_NAME, ORDINAL_POSITION";

            var resultado = new List<TablaInfo>();

            using (var connection = _context.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var tablas = new Dictionary<string, TablaInfo>();

                        while (await reader.ReadAsync())
                        {
                            var nombreTabla = reader.GetString(0);
                            var nombreColumna = reader.GetString(1);
                            var tipoDato = reader.GetString(2);

                            if (!tablas.ContainsKey(nombreTabla))
                            {
                                tablas[nombreTabla] = new TablaInfo
                                {
                                    NombreTabla = nombreTabla,
                                    Columnas = new List<ColumnaInfo>()
                                };
                            }

                            tablas[nombreTabla].Columnas.Add(new ColumnaInfo
                            {
                                NombreColumna = nombreColumna,
                                TipoDato = tipoDato
                            });
                        }

                        resultado = tablas.Values.ToList();
                    }
                }
            }

            return resultado;
        }
    }

}
