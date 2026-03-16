using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos
{
    public class DapperContext
    {
        public readonly string connectionString;

        public DapperContext(IConfiguration config)
        {
            connectionString = config.GetConnectionString("DefaultConnection");
        }
    }
}
