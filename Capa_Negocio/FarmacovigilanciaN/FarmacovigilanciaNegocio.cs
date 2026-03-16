using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.FarmacovigilanciaD;
using Capa_Datos.UsuarioD;
using Capa_Modelo.Farmacovigilancia;
using Capa_Modelo.Login;
using X.PagedList;
using Microsoft.AspNetCore.Mvc;
namespace Capa_Negocio.FarmacovigilanciaN
{
    public interface IFarmacovigilanciaNegocio
    {
        Task ActualizarEstado(CambiarEstadoViewModel estado);
        Task<Farmacovigilancia> BuscarFarmacovigilancia(int IdFarmacovigilancia);
        Task<IPagedList<Farmacovigilancia>> ListarDetallePaginado(string NombrePaciente,  int page = 1, int pageSize = 10);
        Task<List<Farmacovigilancia>> ObtenerFarmacovigilanciaExcel(string nombrePaciente);
    }

    public class FarmacovigilanciaNegocio : IFarmacovigilanciaNegocio
    {
        private readonly IFarmacovigilanciaDatos farmacovigilanciaDatos;

        public FarmacovigilanciaNegocio(IFarmacovigilanciaDatos farmacovigilanciaDatos)
        {
            this.farmacovigilanciaDatos = farmacovigilanciaDatos;
        }

        public async Task<IPagedList<Farmacovigilancia>> ListarDetallePaginado(string NombrePaciente, int page = 1, int pageSize = 10)
        {
            return await farmacovigilanciaDatos.ListarDetallePaginado(NombrePaciente,page, pageSize);
        }


        public async Task ActualizarEstado(CambiarEstadoViewModel estado)
        {
             await farmacovigilanciaDatos.ActualizarEstado(estado);
        }

        public async Task<Farmacovigilancia> BuscarFarmacovigilancia(int IdFarmacovigilancia)
        {
            return await farmacovigilanciaDatos.BuscarFarmacovigilancia(IdFarmacovigilancia);
        }

        public async Task<List<Farmacovigilancia>> ObtenerFarmacovigilanciaExcel(string nombrePaciente)
        {
            return await farmacovigilanciaDatos.ListarDetalleExcel(nombrePaciente ?? string.Empty);
        }

    }
}
