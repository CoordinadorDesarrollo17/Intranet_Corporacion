using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Modelo.Menu
{
    public class MenuSistema
    {
        public int IdMenu { get; set; }
        public string NombreMenu { get; set; }
        public int Nivel { get; set; }
        public int? SuperiorIdMenu { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int Orden { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public List<MenuSistema> SubMenus { get; set; } = new();
    }
}
