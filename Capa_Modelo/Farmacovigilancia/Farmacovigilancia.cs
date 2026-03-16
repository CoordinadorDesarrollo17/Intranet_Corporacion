using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Modelo.Farmacovigilancia
{
    public class Farmacovigilancia
    {

       public int IdFarmacovigilancia { get; set; }

        [Display(Name = "Nombres y Apellidos")]
        public string NombreCompletoPaciente { get; set; }

        [Display(Name = "Edad")]
        public int EdadPaciente { get; set; }

        [Display(Name = "Sexo")]
        public string SexoPaciente { get; set; }

        [Display(Name = "Número de Contacto")]
        public string CelularPaciente { get; set; }

        [Display(Name = "Nombre del Producto")]
        public string NombreProducto { get; set; }

        [Display(Name = "Narración de la reacción adversa")]
        public string DescripcionRAM { get; set; }

        [Display(Name = "Nombre")]
        public string NombreNotificador { get; set; }

        [Display(Name = "Número de Contacto")]
        public string CelularNotificador { get; set; }
        public int Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCreacion { get; set; }

    }
}
