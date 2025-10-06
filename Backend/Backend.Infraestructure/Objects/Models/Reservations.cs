using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Objects.Models
{
    public class Reservations
    {
        public int? id_user {  get; set; }
        public string location {  get; set; }
        public string service_name { get; set; }
        public string frequency { get; set; }
    }
}
