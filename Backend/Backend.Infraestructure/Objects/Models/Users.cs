using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Objects.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int EconomicLevel { get; set; }
        public bool verificate { get; set; }
        public string shelter { get; set; }
        public bool? isAdmin { get; set; }
    }
}
