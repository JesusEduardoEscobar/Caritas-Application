using Backend.Infraestructure.Implementations;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Backend.Infraestructure.Interfaces
{
    public interface IConnectionService
    {
        Task<GlobalResponse<IEnumerable<dynamic>>> VerificarConexionAsync();
    }
}
