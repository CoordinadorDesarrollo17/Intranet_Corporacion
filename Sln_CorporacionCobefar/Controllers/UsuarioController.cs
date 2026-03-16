using Capa_Modelo.Login;
using Capa_Negocio.Identity;
using Capa_Negocio.UsuarioN;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace Sln_CorporacionCobefar.Controllers
{
    // [AllowAnonymous]
    [Authorize(Roles = "MANAGER")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioNegocio usuarioNegocio;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        public UsuarioController(IUsuarioNegocio usuarioNegocio, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            this.usuarioNegocio = usuarioNegocio;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> InactivarUsuario(int idUsuario)
        {
            await usuarioNegocio.ActualizarEstado(idUsuario,0);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ActivarUsuario(int idUsuario)
        {
            await usuarioNegocio.ActualizarEstado(idUsuario, 1);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string NombreUsuario, int page = 1, int pageSize = 10)
        {
            ViewData["NombreUsuario"] = NombreUsuario;
            ViewData["PageSize"] = pageSize;

            var listaUsuarios = await usuarioNegocio
                .ListarUsuarioPaginado(NombreUsuario, page, pageSize);

            return View(listaUsuarios);
        }
        // Para búsquedas con AJAX
        public async Task<IActionResult> BuscarUsuarios(string nombreUsuario, int page = 1, int pageSize = 10)
        {
            ViewData["NombreUsuario"] = nombreUsuario;
            ViewData["PageSize"] = pageSize;

            var listaUsuarios = await usuarioNegocio
                .ListarUsuarioPaginado(nombreUsuario, page, pageSize);

            return PartialView("_TablaUsuarios", listaUsuarios);
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(UsuarioCreacionViewModel usuario)
        {
            if (!ModelState.IsValid)
            {
                var roles = await usuarioNegocio.fObtenerRoles();
                usuario.Roles = roles.Prepend(new SelectListItem { Text = "Seleccionar", Value = "" });
                return View(usuario);
            }

            if (usuario.IdUsuario > 0)
            {
                // 🔹 Obtener usuario
                var userExistente = await userManager.FindByNameAsync(usuario.UsuarioConcatenado);
                if (userExistente == null)
                    return NotFound();

                // 🔹 Actualizar datos NO sensibles
                userExistente.EmailUsuario = usuario.EmailUsuario;

                // 🔐 SOLO si ambos campos tienen valor, se cambia la contraseña
                if (!string.IsNullOrWhiteSpace(usuario.Contraseña) ||
                    !string.IsNullOrWhiteSpace(usuario.RepetirContraseña))
                {
                    // 1️⃣ Ambos campos obligatorios
                    if (string.IsNullOrWhiteSpace(usuario.Contraseña) || string.IsNullOrWhiteSpace(usuario.RepetirContraseña))
                    {

                        ModelState.AddModelError("Contraseña","Debe ingresar contraseña");
                        ModelState.AddModelError("RepetirContraseña", "Debe ingresar contraseña");

                        var roles = await usuarioNegocio.fObtenerRoles();
                        usuario.Roles = roles.Prepend(new SelectListItem { Text = "Seleccionar", Value = "" });
                        usuario.EsEdicion = true;
                        usuario.activo = 1;

                        return View(usuario);
                    }
                  

                    // 2️⃣ Coincidencia
                    if (usuario.Contraseña != usuario.RepetirContraseña)
                    {
                        ModelState.AddModelError("Contraseña", "Las contraseñas no coinciden.");
                        ModelState.AddModelError("RepetirContraseña","Las contraseñas no coinciden.");

                        var roles = await usuarioNegocio.fObtenerRoles();
                        usuario.Roles = roles.Prepend(new SelectListItem { Text = "Seleccionar", Value = "" });
                        usuario.EsEdicion = true;
                        usuario.activo = 1;

                        return  View(usuario);
                    }

                    // 3️⃣ Validación mínima (UX)
                    if (usuario.Contraseña.Length < 6 || usuario.RepetirContraseña.Length <6 )
                    {
                        ModelState.AddModelError("Contraseña","La contraseña debe tener al menos 6 caracteres.");
                        ModelState.AddModelError("RepetirContraseña", "La contraseña debe tener al menos 6 caracteres.");

                        var roles = await usuarioNegocio.fObtenerRoles();
                        usuario.Roles = roles.Prepend(new SelectListItem { Text = "Seleccionar", Value = "" });
                        usuario.EsEdicion = true;
                        usuario.activo = 1;

                        return View(usuario);
                    }

                    // 4️⃣ Cambio REAL de contraseña (Identity)
                    var token = await userManager.GeneratePasswordResetTokenAsync(userExistente);
                    var result = await userManager.ResetPasswordAsync(
                        userExistente, token, usuario.Contraseña);

                    // 5️⃣ Mostrar errores reales de Identity
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("Contraseña", error.Description);

                        return  View(usuario);
                    }
                }

                // 🔹 Guardar otros cambios
                await userManager.UpdateAsync(userExistente);

                TempData["Mensaje"] = "Usuario actualizado correctamente";
            }
            else
            {
                // 🔹 Creación de nuevo usuario
                usuario.PasswordHash = (usuario.NombresUsuario.Substring(0, 3) + usuario.ApellidosUsuario.Substring(0, 3)).ToUpper() + usuario.SecuenciaPorRol;
                usuario.activo = 1;
                usuario.FechaRegistro = DateTime.Today;
                usuario.HoraRegistro = DateTime.Now.TimeOfDay;
                usuario.FechaUltimoIngreso = null;
                usuario.UsuarioRegistro = User.FindFirst("NombreCompleto")?.Value;

                var resultado = await userManager.CreateAsync(usuario, usuario.PasswordHash);

                if (!resultado.Succeeded)
                {
                    foreach (var error in resultado.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    var roles = await usuarioNegocio.fObtenerRoles();
                    usuario.Roles = roles.Prepend(new SelectListItem { Text = "Seleccionar", Value = "" });
                    return View(usuario);
                }

                TempData["NuevoUsuario"] = usuario.UsuarioConcatenado;
                TempData["NuevaPassword"] = (usuario.NombresUsuario.Substring(0, 3) + usuario.ApellidosUsuario.Substring(0, 3)).ToUpper() + usuario.SecuenciaPorRol;
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> CrearUsuario(string? usuario)
        {
            var modelo = new UsuarioCreacionViewModel();

            // Obtener roles
            var roles = await usuarioNegocio.fObtenerRoles();
            modelo.Roles = roles.Prepend(new SelectListItem
            {
                Text = "Seleccionar",
                Value = ""
            });

            if (usuario != null)
            {
                // Traer datos del usuario existente
                var usuarioDb = await usuarioNegocio.BuscarUsuario(usuario);
                if (usuarioDb == null)
                    return NotFound();

                // Mapear los datos al ViewModel
                modelo.IdUsuario = usuarioDb.IdUsuario;
                modelo.NombresUsuario = usuarioDb.NombresUsuario;
                modelo.ApellidosUsuario = usuarioDb.ApellidosUsuario;
                modelo.EmailUsuario = usuarioDb.EmailUsuario;
                modelo.IdRol = usuarioDb.IdRol;
                modelo.UsuarioConcatenado = usuarioDb.UsuarioConcatenado;
                modelo.PrefijoRol = usuarioDb.PrefijoRol;
                modelo.SecuenciaPorRol = usuarioDb.SecuenciaPorRol;

                modelo.activo = usuarioDb.activo;
                modelo.FechaRegistro = usuarioDb.FechaRegistro;

                //modelo.Contraseña = (usuarioDb.NombresUsuario.Substring(0, 3) + usuarioDb.ApellidosUsuario.Substring(0, 3)).ToUpper() + usuarioDb.SecuenciaPorRol;
                //modelo.RepetirContraseña = (usuarioDb.NombresUsuario.Substring(0, 3) + usuarioDb.ApellidosUsuario.Substring(0, 3)).ToUpper() + usuarioDb.SecuenciaPorRol;

                modelo.EsEdicion = true;
            }
            else
            {
                modelo.EsEdicion = false;
            }

                return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarioPorRol(int idRol)
        {
            var usuario = await usuarioNegocio.fObtenerUsuarioPorRol(idRol);
            return Json(usuario);
        }


    }
}
