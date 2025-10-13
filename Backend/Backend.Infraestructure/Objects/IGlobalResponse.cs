using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Infraestructure.Objects
{
    public interface IGlobalResponse<T>
    {
        bool Ok { get; set; }
        int RowsCount { get; set; }
        string Code { get; set; }
        string Message { get; set; }

        T? Data { get; set; }
    }
}