using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Infraestructure.Implementations;

namespace Backend.Interfaces
{
    public interface IAuthenticator
    {
        Task<GlobalResponse<dynamic>> Login(string email, string password);

    }
}
