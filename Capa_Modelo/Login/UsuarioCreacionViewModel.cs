using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Capa_Modelo.Login
{
    public class UsuarioCreacionViewModel : Usuario
    {
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
