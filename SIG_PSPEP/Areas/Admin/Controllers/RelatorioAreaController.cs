using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;
using System.Data;
using System.Drawing;

namespace SIG_PSPEP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrador")]
    public class RelatorioAreaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly AppDbContext _context;

        public RelatorioAreaController(IWebHostEnvironment webHostEnv, AppDbContext context)
        {
            _webHostEnv = webHostEnv;
            _context = context;
        }

        [HttpGet]
        public IActionResult CriarModelo()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportArea.frx");

            if (System.IO.File.Exists(caminhoReport))
                return Ok("O modelo já foi criado anteriormente.");

            var report = new Report();
            var listaAreas = _context.Areas.ToList();

            report.Dictionary.RegisterBusinessObject(listaAreas, "ListaAreas", 10, true);
            report.Report.Save(caminhoReport);

            return Ok("Modelo de relatório criado e salvo.");
        }

        [HttpGet]
        public IActionResult ExportarPDF()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportArea.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado.");

            var listaAreas = _context.Areas.ToList();

            using var report = new Report();
            report.Load(caminhoReport);
            report.Dictionary.RegisterBusinessObject(listaAreas, "ListaAreas", 10, true);
            report.Prepare();

            using var ms = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, ms);
            ms.Position = 0;

            return File(ms.ToArray(), "application/pdf", "Relatorio_Areas.pdf");
        }

        [HttpGet]
        public IActionResult VisualizarPDF()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportArea.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado.");

            var listaAreas = _context.Areas.ToList();

            var webReport = new WebReport();
            webReport.Report.Load(caminhoReport);
            webReport.Report.Dictionary.RegisterBusinessObject(listaAreas, "ListaAreas", 10, true);
            webReport.Report.Prepare();
            webReport.ShowToolbar = true;
            webReport.ShowPrint = true;
            webReport.ShowRefreshButton = true;

            return PartialView("_VisualizarPDF", webReport);
        }
    }
}
