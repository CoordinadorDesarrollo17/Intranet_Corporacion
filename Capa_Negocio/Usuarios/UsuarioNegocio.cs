using Capa_Datos.UsuarioD;
using Capa_Modelo.Login;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Capa_Negocio.UsuarioN
{

    public interface IUsuarioNegocio
    {
        Task<int> ActualizarEstado(int IdUsuario, int activo);
        Task<Usuario> BuscarUsuario(string usuario);
        Task<IEnumerable<SelectListItem>> fObtenerRoles();
        Task<UsuarioPorRol> fObtenerUsuarioPorRol(int idRol);
        Task<IPagedList<Usuario>> ListarUsuarioPaginado(string NombreUsuario, int page = 1, int pageSize = 10);
    }
    public class UsuarioNegocio : IUsuarioNegocio
    {
        private readonly IUsuarioDatos usuarioDatos;

        public UsuarioNegocio(IUsuarioDatos usuarioDatos )
        {
            this.usuarioDatos = usuarioDatos;
        }

        public async Task<IEnumerable<SelectListItem>> fObtenerRoles()
        {
            return await usuarioDatos.fObtenerRoles();
        }

        public async Task<UsuarioPorRol> fObtenerUsuarioPorRol(int idRol)
        {
            return await usuarioDatos.fObtenerUsuarioPorRol(idRol);
        }
        public async Task<IPagedList<Usuario>> ListarUsuarioPaginado(string NombreUsuario, int page = 1, int pageSize = 10)
        {
            return await usuarioDatos.ListarUsuarioPaginado(NombreUsuario, page, pageSize);
        }

        public async Task<Usuario> BuscarUsuario(string usuario)
        {
            return await usuarioDatos.BuscarUsuario(usuario);
        }


        public async Task<int> ActualizarEstado(int IdUsuario, int activo)
        {
            return await usuarioDatos.ActualizarEstado(IdUsuario, activo);
        }

    }
}
