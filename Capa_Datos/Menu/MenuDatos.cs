using Capa_Modelo.Login;
using Capa_Modelo.Menu;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Menu
{

    public interface IMenuDatos
    {
        Task<IEnumerable<MenuSistema>> fObtenerMenuPorRol(int idRol);
    }
    public class MenuDatos : IMenuDatos
    {
        private readonly DapperContext dapperContext;

        public MenuDatos(DapperContext dapperContext) {
            this.dapperContext = dapperContext;
        }

        public async Task<IEnumerable<MenuSistema>> fObtenerMenuPorRol(int idRol)
        {
            using var xCon = new SqlConnection(dapperContext.connectionString);

            string sql;

            if (idRol == 0)
            {
                // Traer todos los registros si idRol es 0
                sql = @"
            SELECT *
            FROM menu_sistema
            ORDER BY Orden";
                return await xCon.QueryAsync<MenuSistema>(sql);
            }
            else
            {
                // Filtrar por IdRol si es distinto de 0
                sql = @"
            SELECT *
            FROM menu_sistema
            WHERE IdRol = @idRol
            ORDER BY Orden";
                return await xCon.QueryAsync<MenuSistema>(sql, new { idRol });
            }
        }



    }
}
