using Capa_Modelo.Login;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Capa_Datos.UsuarioD
{
    public interface IUsuarioDatos
    {
        Task<int> ActualizarEstado(int IdUsuario, int activo);
        Task<int> ActualizarUsuario(Usuario usuario);
        Task<Usuario> BuscarUsuario(string usuario);
        Task<int> CrearUsuario(Usuario usuario);
        Task<IEnumerable<SelectListItem>> fObtenerRoles();
        Task<UsuarioPorRol> fObtenerUsuarioPorRol(int idRol);
        Task<IPagedList<Usuario>> ListarUsuarioPaginado(string NombreUsuario, int page = 1, int pageSize = 10);
    }

    public class UsuarioDatos : IUsuarioDatos
    {
        private readonly DapperContext dapperContext;

        public UsuarioDatos(DapperContext dapperContext)
        {
            this.dapperContext = dapperContext;
        }

        public async Task<IPagedList<Usuario>> ListarUsuarioPaginado(string NombreUsuario, int page = 1, int pageSize = 10)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);
            var result = await xCon.QueryAsync<Usuario>(
                              @"SELECT IdUsuario, 
                                     (PrefijoRol + SecuenciaPorRol) AS UsuarioConcatenado,
                                     NombresUsuario ,ApellidosUsuario,
                                     PrefijoRol, FechaRegistro, UsuarioRegistro, usu.Activo 
                              FROM seguridad.usuarios usu
                              WHERE (NombresUsuario + ApellidosUsuario + (PrefijoRol + SecuenciaPorRol) + PrefijoRol + ISNULL(UsuarioRegistro,'')) LIKE @NombreUsuario 
                              ORDER BY IdUsuario DESC",
      new { NombreUsuario = "%" + NombreUsuario + "%" } // <-- comodines
  );
            var listaPaginada = result.ToPagedList(page, pageSize);
            return listaPaginada;
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
   
            using var xCon = new SqlConnection(dapperContext.connectionString);
         
            var usuarioId = await xCon.QuerySingleAsync<int>(@"
                        INSERT INTO seguridad.usuarios (NombresUsuario,ApellidosUsuario,EmailUsuario,IdRol,PrefijoRol,SecuenciaPorRol,PasswordHash,Activo,UsuarioRegistro,FechaRegistro,HoraRegistro,FechaUltimoIngreso)
                        VALUES (@NombresUsuario,@ApellidosUsuario,@EmailUsuario,@IdRol,@PrefijoRol,@SecuenciaPorRol,@PasswordHash,@Activo,@UsuarioRegistro,@FechaRegistro,@HoraRegistro,@FechaUltimoIngreso);
                        SELECT SCOPE_IDENTITY();", usuario);

            return usuarioId;
        }
        public async Task<int> ActualizarUsuario(Usuario usuario)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var query = @"
        UPDATE seguridad.usuarios
        SET 
            EmailUsuario = @EmailUsuario,
            PasswordHash = @PasswordHash
        WHERE IdUsuario = @IdUsuario;";

            var filas = await xCon.ExecuteAsync(query, usuario);
            return filas;
        }


        public async Task<int> ActualizarEstado(int IdUsuario, int activo)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var query = @"
        UPDATE seguridad.usuarios
        SET Activo = @activo
        WHERE IdUsuario = @IdUsuario";

            return await xCon.ExecuteAsync(query, new { IdUsuario, activo });
        }



        public async Task<IEnumerable<SelectListItem>> fObtenerRoles() 
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);
         
            var listaRoles = await xCon.QueryAsync<Rol>(@"SELECT * FROM [seguridad].[roles] ORDER BY IdRol");

            //SelectListItem es una clase de ASP.NET que representa un ítem en una etiqueta <select> HTML, se usa de la sgt manera  SelectListItem(Text (lo que ve el usuario),Value (valor que se asigna cuando seleccionamos una opcion))
            return listaRoles.Select(x => new SelectListItem(x.NombreRol, x.IdRol.ToString())); // transformamos cada objeto x en un SelectListItem.
        }

        public async Task<UsuarioPorRol> fObtenerUsuarioPorRol(int idRol)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            var sql = @"SELECT TOP 1 (UPPER(PrefijoRol) +  CONVERT(VARCHAR,(SecuenciaPorRol+1)) ) AS usuario, UPPER(PrefijoRol) AS PrefijoRol , (SecuenciaPorRol + 1) AS SecuenciaPorRol
                FROM seguridad.usuarios
                WHERE IdRol = @IdRol ORDER BY SecuenciaPorRol DESC ";

            var result = await xCon.QueryFirstOrDefaultAsync<UsuarioPorRol>(sql, new { idRol });
            
            if (result == null)
            {
                var sql2 = @"SELECT (UPPER(PrefijoRol)+'1') AS usuario,UPPER(PrefijoRol) AS PrefijoRol , '1' AS SecuenciaPorRol FROM seguridad.roles WHERE IdRol = @IdRol";
                return await xCon.QueryFirstOrDefaultAsync<UsuarioPorRol>(sql2, new { idRol });
            }
            else
            {
                return result;
            }
        }


        public async Task<Usuario> BuscarUsuario(string usuario)
        {
            //QuerySingleOrDefaultAsync
            //Devuelve un solo registro si existe, sino devuelve por defecto null
            //Lanza excepción si hay más de una fila

            using var xCon = new SqlConnection(dapperContext.connectionString);
            var usuarioEncontrado  = await xCon.QuerySingleOrDefaultAsync<Usuario>(@"SELECT 
IdUsuario,NombresUsuario,ApellidosUsuario,EmailUsuario, IdRol,SecuenciaPorRol,PrefijoRol ,PasswordHash,Activo,UsuarioRegistro,FechaRegistro,HoraRegistro,FechaUltimoIngreso ,
(TRIM(PrefijoRol)+TRIM(SecuenciaPorRol)) AS UsuarioConcatenado
FROM seguridad.usuarios WHERE TRIM(PrefijoRol)+TRIM(SecuenciaPorRol)=@Usuario ", new { usuario });

            return usuarioEncontrado;

        }


    }
}
