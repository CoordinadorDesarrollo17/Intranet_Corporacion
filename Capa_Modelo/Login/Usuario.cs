using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Modelo.Login
{
    public class Usuario
    {
      public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombres")]
        public string NombresUsuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Apellidos")]
        public string ApellidosUsuario { get; set; }

        [Display(Name = "Email")]
        public string? EmailUsuario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }
        public string? SecuenciaPorRol { get; set; }
        public string? PrefijoRol { get; set; }
        public string? PasswordHash { get; set; }
        public int? activo { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Usuario")]
        public string UsuarioConcatenado { get; set; }

        public string? UsuarioRegistro { get; set; } 
        public DateTime? FechaRegistro { get; set; }
        public TimeSpan? HoraRegistro { get; set; }
        public DateTime? FechaUltimoIngreso { get; set; }

        [Display(Name = "Contraseña")]
        public string? Contraseña { get; set; }

        [Display(Name = "Repetir Contraseña")]
        public string? RepetirContraseña { get; set; }

        public bool EsEdicion { get; set; } = false;
    }
}
