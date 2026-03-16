using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Modelo.Farmacovigilancia
{
    public class CambiarEstadoViewModel
    {

        public int IdFarmacovigilancia { get; set; }
        public int Estado { get; set; }

        public string? UsuarioModificacion { get; set; }

        public DateTime? FechaModificacion { get; set; }   

        public TimeSpan? HoraModificacion { get; set; }
    }
}
