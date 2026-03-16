using System.Security.Claims;
using Capa_Modelo.Login;
using Capa_Negocio.UsuarioN;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Sln_CorporacionCobefar.Controllers
{
    [AllowAnonymous] //En este metodo Excluimos la politica de autenticacion creada en Program.cs
    public class LoginController : Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly IUsuarioNegocio usuarioNegocio;

        public LoginController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IUsuarioNegocio usuarioNegocio)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.usuarioNegocio = usuarioNegocio;
        }



        [HttpGet]
        public IActionResult Login()
        {
          return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuarioDb = await usuarioNegocio.BuscarUsuario(modelo.usuario);
            if (usuarioDb == null)
            {
                ModelState.AddModelError("", "Nombre de usuario incorrecto.");
                return View(modelo);
            }
            if (usuarioDb.activo == 0)
            {
                ModelState.AddModelError("", "El usuario se encuentra inactivo.");
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.usuario,modelo.password,true,lockoutOnFailure: false);
           
            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("", "Nombre de usuario o password incorrecto.");
                return View(modelo);
            }
            var user = await userManager.FindByNameAsync(modelo.usuario);
            var claims = new List<Claim>
            {
                new Claim("NombreCompleto", usuarioDb?.NombresUsuario + " " + usuarioDb?.ApellidosUsuario ?? modelo.usuario),
                new Claim("IdRol", usuarioDb.IdRol.ToString()),
                new Claim(ClaimTypes.Role, usuarioDb.PrefijoRol)
            };

            await signInManager.SignOutAsync(); //limpiamos claims
            await signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims); //agregamos claims

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme); //Este código cierra la sesión del usuario actual eliminando su cookie de autenticación de Identity

            return RedirectToAction("Index", "Home");
        }

    }
}
