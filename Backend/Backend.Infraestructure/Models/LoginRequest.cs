using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Models
{
    // Modelo de request para el login
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    // Modelo de request para saber si el usuario esta verificado (registrado de forma completa)
    public class VerifyUserRequest
    {
        public int Id { get; set; }
        public bool Verificacion { get; set; }
    }

    public class DeleteUserRequesst
    {
        public int Id { get; set; }
    }
    
    public class EditUserRequest
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Numero { get; set; }
        public int? ShelterId { get; set; }
        public bool? Verificado { get; set; }
        public string? NivelEconomico { get; set; }
    }

}
