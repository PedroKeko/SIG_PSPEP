using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    public class RelatorioController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly AppDbContext _context;

        public RelatorioController(IWebHostEnvironment webHostEnv, AppDbContext context)
        {
            _webHostEnv = webHostEnv;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CriarModelo()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportEfectivo.frx");

            if (System.IO.File.Exists(caminhoReport))
                return Ok("O modelo já foi criado anteriormente.");

            var report = new Report();

            // Consulta com Include igual ao Index
            var listaEfectivos = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.Municipio)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.ProvinciaNascimento)
                .Include(e => e.ProvinciaResidencia)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .OrderByDescending(e => e.Patente.grau)
                .ThenBy(e => e.NomeCompleto)
                .ToListAsync();

            // Registrar como data source para o FastReport
            report.Dictionary.RegisterBusinessObject(listaEfectivos, "ListaEfectivos", 10, true);

            // Salvar o modelo
            report.Report.Save(caminhoReport);

            return Ok("Modelo de relatório criado com sucesso.");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarPDF()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportEfectivo.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado.");

            var listaEfectivos = await _context.Efectivos
               .Include(e => e.FuncaoCargo)
               .Include(e => e.Municipio)
               .Include(e => e.OrgaoUnidade)
               .Include(e => e.Patente)
               .Include(e => e.ProvinciaNascimento)
               .Include(e => e.ProvinciaResidencia)
               .Include(e => e.SituacaoEfectivo)
               .Include(e => e.User)
               .OrderByDescending(e => e.Patente.grau)
               .ThenBy(e => e.NomeCompleto)
               .ToListAsync();


            using var report = new Report();
            report.Load(caminhoReport);
            report.Dictionary.RegisterBusinessObject(listaEfectivos, "ListaEfectivo", 10, true);
            report.Prepare();

            using var ms = new MemoryStream();
            var pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, ms);
            ms.Position = 0;

            return File(ms.ToArray(), "application/pdf", "Relatorio_Efectivos.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarPDF()
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\ReportEfectivo.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado.");

            var listaEfectivos = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.Municipio)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.ProvinciaNascimento)
                .Include(e => e.ProvinciaResidencia)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .OrderByDescending(e => e.Patente.grau)
                .ThenBy(e => e.NomeCompleto)
                .ToListAsync();

            var webReport = new WebReport();
            webReport.Report.Load(caminhoReport);

            // ✅ Registrar como BusinessObject (suporta entidades EF)
            webReport.Report.Dictionary.RegisterBusinessObject(listaEfectivos, "ListaEfectivos", 10, true);

            // ✅ Mostra barra de ferramentas do visualizador
            webReport.ShowToolbar = true;
            webReport.ShowPrint = true;
            webReport.ShowRefreshButton = true;

            return PartialView("_VisualizarPDF", webReport);
        }

    }
}
