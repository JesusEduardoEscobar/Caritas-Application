using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Extensions
{
    public class HConstants
    {
        public const string NameQuery_Config = "Extract General Configurations";

        public const string Header_Token = "Token";
        public const string POST = "POST";
        public const string PATCH = "PATCH";
        public const string Pendiente = "Pendiente";
        public const string Autorizado = "Autorizado";
        public const string Rechazada = "Rechazada";
        public const string Error = "Error";
        public const int SLPostOK = 201;
        public const int SLPatchOK = 204;

        public const int PagesSize = 500;
        public const int PagesSize2000 = 2000;
    }
}
