using Capa_Negocio.Menu;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Sln_CorporacionCobefar.Views.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuNegocio menuNegocio;

        public MenuViewComponent(IMenuNegocio menuNegocio) {
            this.menuNegocio = menuNegocio;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(string.Empty);

            var idRolClaim = HttpContext.User.FindFirst("IdRol")?.Value;

            if (!int.TryParse(idRolClaim, out int idRol))
                return Content(string.Empty);

            var menu = await menuNegocio.ObtenerMenuJerarquicoPorRol(idRol);

            return View(
                "~/Views/Shared/Components/Menu/MenuDefault.cshtml",
                menu
            );
        }
    }

}
