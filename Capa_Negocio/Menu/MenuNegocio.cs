using Capa_Datos.Menu;
using Capa_Modelo.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Menu
{
    public interface IMenuNegocio
    {
        Task<List<MenuSistema>> ObtenerMenuJerarquicoPorRol(int idRol);
    }

    public class MenuNegocio : IMenuNegocio
    {
        private readonly IMenuDatos menuDatos;

        public MenuNegocio(IMenuDatos menuDatos )
        {
            this.menuDatos = menuDatos;
        }

        public async Task<List<MenuSistema>> ObtenerMenuJerarquicoPorRol(int idRol)
        {
            if(idRol ==1)
            {
                idRol = 0;
            }

            var menus = (await menuDatos.fObtenerMenuPorRol(idRol)).ToList();

            var lookup = menus.ToLookup(m => m.SuperiorIdMenu);

            foreach (var menu in menus)
            {
                menu.SubMenus = lookup[menu.IdMenu]
                                .OrderBy(x => x.Orden)
                                .ToList();
            }

            foreach (var menu in menus)
            {
                if (!menu.SubMenus.Any())
                    ProcesarRuta(menu);

                foreach (var sub in menu.SubMenus)
                    ProcesarRuta(sub);
            }

            return menus
                .Where(m => m.SuperiorIdMenu == null)
                .OrderBy(m => m.Orden)
                .ToList();
        }


        private void ProcesarRuta(MenuSistema menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Ruta))
            {
                // Opcional: puedes asignar Home/Index por default
                menu.Controller = "Home";
                menu.Action = "Index";
                return;
            }

            var partes = menu.Ruta.Split('/');
            menu.Controller = partes[0];
            menu.Action = partes.Length > 1 ? partes[1] : "Index";
        }



    }
}
