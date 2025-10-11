using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Models
{
    // Modelo de request para el login
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Modelo de request para saber si el usuario esta verificado (registrado de forma completa)
    public class VerifyUserRequest
    {
        public int Id { get; set; }
        public bool Verificacion { get; set; }
    }

}
