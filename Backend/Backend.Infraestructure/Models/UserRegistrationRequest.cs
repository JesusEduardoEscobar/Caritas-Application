using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Models
{
    public class UserRegistrationRequest
    {
        public string? Nombre { get; set; }
        [EmailAddress]
        public string Email { get; set; } // opcional para registro lite
        public string? Password { get; set; }
        public string Numero { get; set; } // puede ser teléfono o shelter
        public int ShelterId { get; set; } // opcional
        public string? NivelEconomico { get; set; } // opcional
        public bool? AntecedentesNoPenales { get; set; } // opcional
        public bool? Verificacion { get; set; } // opcional
        public DateTime FechaDeNacimiento { get; set; }
    }
    
    public class AdminRegistrationRequest
    {
        public string Nombre { get; set; }
        [EmailAddress]
        public string Email { get; set; } // opcional para registro lite
        public string Password { get; set; }
        public bool? Verificacion { get; set; } // opcional
        [EmailAddress]
        public string? EmailAdmin { get; set; } // solo para registro admin
        public string? PasswordAdmin { get; set; } // solo para registro admin
    }
}
