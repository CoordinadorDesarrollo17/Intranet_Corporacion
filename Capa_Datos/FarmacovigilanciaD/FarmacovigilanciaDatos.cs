using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Modelo.Login;
using Dapper;
using Microsoft.Data.SqlClient;
using X.PagedList;
using X.PagedList.Extensions;
using Capa_Modelo.Farmacovigilancia;

namespace Capa_Datos.FarmacovigilanciaD
{
    public interface IFarmacovigilanciaDatos
    {
        Task ActualizarEstado(CambiarEstadoViewModel estado);
        Task<Farmacovigilancia> BuscarFarmacovigilancia(int IdFarmacovigilancia);
        Task<List<Farmacovigilancia>> ListarDetalleExcel(string NombrePaciente);
        Task<IPagedList<Farmacovigilancia>> ListarDetallePaginado(string NombrePaciente, int page = 1, int pageSize = 10);
    }

    public class FarmacovigilanciaDatos : IFarmacovigilanciaDatos
    {
        private readonly DapperContext dapperContext;

        public FarmacovigilanciaDatos(DapperContext dapperContext)
        {
            this.dapperContext = dapperContext;
        }


        public async Task<IPagedList<Farmacovigilancia>> ListarDetallePaginado(string NombrePaciente,int page = 1,int pageSize = 10)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var sql = @"
                SELECT IdFarmacovigilancia
                      ,pac.NombreCompletoPaciente
                      ,pac.SexoPaciente   
                      ,pro.NombreProducto 
                      ,ntf.NombreNotificador
                      ,Estado
                      ,FechaCreacion
                FROM [CORPORACION_COBEFAR].[farm].[detalle_farmacovigilancia] AS dfa
                LEFT JOIN farm.paciente AS pac ON pac.IdPaciente = dfa.IdPaciente 
                LEFT JOIN farm.producto AS pro ON pro.IdProducto = dfa.IdProducto 
                LEFT JOIN farm.notificador AS ntf ON ntf.IdNotificador  = dfa.IdNotificador
                WHERE pac.NombreCompletoPaciente + pro.NombreProducto + ntf.NombreNotificador + CONVERT(varchar(10), FechaCreacion, 103)  LIKE @NombrePaciente";
         
                 sql += " ORDER BY IdFarmacovigilancia DESC";

            var result = await xCon.QueryAsync<Farmacovigilancia>(sql,new{NombrePaciente = "%" + NombrePaciente + "%"});

            var listaPaginada = result.ToPagedList(page, pageSize);
            return listaPaginada;
        }


        public async Task<List<Farmacovigilancia>> ListarDetalleExcel(string NombrePaciente)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var sql = @"
        SELECT IdFarmacovigilancia
              ,pac.NombreCompletoPaciente
              ,pac.SexoPaciente   
              ,pro.NombreProducto 
              ,ntf.NombreNotificador
              ,Estado
              ,FechaCreacion
        FROM [CORPORACION_COBEFAR].[farm].[detalle_farmacovigilancia] AS dfa
        LEFT JOIN farm.paciente AS pac ON pac.IdPaciente = dfa.IdPaciente 
        LEFT JOIN farm.producto AS pro ON pro.IdProducto = dfa.IdProducto 
        LEFT JOIN farm.notificador AS ntf ON ntf.IdNotificador  = dfa.IdNotificador
        WHERE pac.NombreCompletoPaciente 
              + pro.NombreProducto 
              + ntf.NombreNotificador 
              + CONVERT(varchar(10), FechaCreacion, 103) LIKE @NombrePaciente
        ORDER BY IdFarmacovigilancia DESC";

            var result = await xCon.QueryAsync<Farmacovigilancia>(
                sql,
                new { NombrePaciente = "%" + NombrePaciente + "%" }
            );

            return result.ToList();
        }


        public async Task ActualizarEstado(CambiarEstadoViewModel estado) 
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var query = @"
        UPDATE farm.detalle_farmacovigilancia 
        SET 
            Estado = @Estado,
UsuarioModificacion=@UsuarioModificacion,
FechaModificacion=@FechaModificacion,
HoraModificacion=@HoraModificacion
        WHERE IdFarmacovigilancia = @IdFarmacovigilancia;";

            await xCon.ExecuteAsync(query, estado);
           
     
        }


        public async Task<Farmacovigilancia> BuscarFarmacovigilancia(int IdFarmacovigilancia)
        {
      
            using var xCon = new SqlConnection(dapperContext.connectionString);
            var datoEncontrado = await xCon.QuerySingleOrDefaultAsync<Farmacovigilancia>(@"SELECT 
           IdFarmacovigilancia
	  ,pac.NombreCompletoPaciente
      ,pac.EdadPaciente
	  ,pac.SexoPaciente   
      ,pac.CelularPaciente
	  ,pro.NombreProducto 
      ,pro.DescripcionRAM
	  ,ntf.NombreNotificador
      ,ntf.CelularNotificador
      ,Estado
      ,FechaCreacion
       FROM [farm].[detalle_farmacovigilancia] AS dfa
       LEFT JOIN farm.paciente AS pac ON pac.IdPaciente = dfa.IdPaciente 
       LEFT JOIN farm.producto AS pro ON pro.IdProducto = dfa.IdProducto 
	   LEFT JOIN farm.notificador AS ntf ON ntf.IdNotificador  = dfa.IdNotificador
                              WHERE IdFarmacovigilancia = @IdFarmacovigilancia
                              ORDER BY IdFarmacovigilancia DESC", new { IdFarmacovigilancia });

            return datoEncontrado;

        }





    }
}
