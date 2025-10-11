using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Models
{
    public class UserRegistrationRequest
    {
        public string Nombre { get; set; }
        public string Email { get; set; } // opcional para registro lite
        public string Password { get; set; }
        public string Numero { get; set; } // puede ser teléfono o shelter
        public string NivelEconomico { get; set; } // opcional
        public bool? Verificacion { get; set; } // opcional
        public string EmailAdmin { get; set; } // solo para registro admin
        public string PasswordAdmin { get; set; } // solo para registro admin
    }
}
