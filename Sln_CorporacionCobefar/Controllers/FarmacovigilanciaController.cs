using Capa_Modelo.Farmacovigilancia;
using Capa_Modelo.Login;
using Capa_Negocio.FarmacovigilanciaN;
using Capa_Negocio.UsuarioN;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Sln_CorporacionCobefar.Controllers
{
    public class FarmacovigilanciaController : Controller
    {
        private readonly IFarmacovigilanciaNegocio farmacovigilanciaNegocio;

        public FarmacovigilanciaController(IFarmacovigilanciaNegocio farmacovigilanciaNegocio)
        {
            this.farmacovigilanciaNegocio = farmacovigilanciaNegocio;
        }

        public async Task<IActionResult> Index(string NombrePaciente,  int page = 1, int pageSize = 10)
        {
            ViewData["NombreUsuario"] = NombrePaciente;
            ViewData["PageSize"] = pageSize;

            var listaDetalle = await farmacovigilanciaNegocio.ListarDetallePaginado(NombrePaciente,  page, pageSize);

            return View(listaDetalle);
        }

        // Para búsquedas con AJAX
        public async Task<IActionResult> BuscarFarmacovigilancia(string NombrePaciente,  int page = 1, int pageSize = 10)
        {
            ViewData["NombreFarmacovigilancia"] = NombrePaciente;
            ViewData["PageSize"] = pageSize;

            var listaUsuarios = await farmacovigilanciaNegocio.ListarDetallePaginado(NombrePaciente, page, pageSize);

            return PartialView("_TablaFarmacovigilancia", listaUsuarios);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoViewModel model)
        {
            try
            {
                model.UsuarioModificacion = User.Identity.Name;
                model.FechaModificacion = DateTime.Today;
                model.HoraModificacion = DateTime.Now.TimeOfDay;

                await  farmacovigilanciaNegocio.ActualizarEstado(model);

                return Json(new { ok = true });
            }
            catch
            {
                return Json(new { ok = false, mensaje = "Error al actualizar estado" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarFarmacovigilancia(int IdFarmacovigilancia)
        {
            var detalle = await farmacovigilanciaNegocio.BuscarFarmacovigilancia(IdFarmacovigilancia);

            if (detalle == null)
            {
                return NotFound();
            }

            return View(detalle);
        }


public async Task<IActionResult> ExportarExcel(string nombrePaciente)
    {
        var lista = await farmacovigilanciaNegocio.ObtenerFarmacovigilanciaExcel(nombrePaciente ?? string.Empty);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Farmacovigilancia");

        // CABECERAS
        ws.Cell(1, 1).Value = "DocEntry";
        ws.Cell(1, 2).Value = "Paciente";
        ws.Cell(1, 3).Value = "Sexo";
        ws.Cell(1, 4).Value = "Producto";
        ws.Cell(1, 5).Value = "Notificador";
        ws.Cell(1, 6).Value = "Fecha Creación";
        ws.Cell(1, 7).Value = "Estado";

        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightGray;

        int row = 2;

        foreach (var x in lista)
        {
            ws.Cell(row, 1).Value = x.IdFarmacovigilancia;
            ws.Cell(row, 2).Value = x.NombreCompletoPaciente;
            ws.Cell(row, 3).Value = x.SexoPaciente;
            ws.Cell(row, 4).Value = x.NombreProducto;
            ws.Cell(row, 5).Value = x.NombreNotificador;
            ws.Cell(row, 6).Value = x.FechaCreacion;
            ws.Cell(row, 6).Style.DateFormat.Format = "dd/MM/yyyy";

            ws.Cell(row, 7).Value =
                x.Estado == 0 ? "ABIERTO" :
                x.Estado == 1 ? "EN PROCESO" :
                "CERRADO";

            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Farmacovigilancia_{DateTime.Now:yyyyMMddHHmm}.xlsx"
        );
    }




}
}
