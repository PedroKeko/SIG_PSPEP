using FastReport;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Areas.Dtti.Models;
using SIG_PSPEP.Context;
using System.Composition;
using System.Data;

namespace SIG_PSPEP.Areas.Dtti.Controllers
{
    [Area("DTTI")]
    public class RelatorioController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly AppDbContext _context;

        public RelatorioController(IWebHostEnvironment webHostEnv, AppDbContext context)
        {
            _webHostEnv = webHostEnv;
            _context = context;
        }
        public async Task<IActionResult> VisualizarRelatorio(int id)
        {
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\RadioGuia.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado. Crie-o primeiro.");

            // Buscar a guia
            var guia = await (
                from g in _context.RadioGuias
                join userAut in _context.UsuarioAutes.Include(u => u.User)
                    on g.UserId equals userAut.UserId into criadorJoin
                from userAut in criadorJoin.DefaultIfEmpty()
                join chefeAut in _context.UsuarioAutes.Include(u => u.User)
                    on g.ChefeId equals chefeAut.UserId into chefeJoin
                from chefeAut in chefeJoin.DefaultIfEmpty()
                where g.Id == id
                select new
                {
                    g.Id,
                    g.NumGuia,
                    g.DataRegisto,
                    g.Aprovado,
                    g.DataAprovacao,
                    NomeUsuario = userAut != null && userAut.User != null ? userAut.User.UserName : "N/A",
                    NomeChefe = chefeAut != null && chefeAut.User != null ? chefeAut.Efectivo.NomeCompleto : "N/A",
                    PatenteChefe = chefeAut != null && chefeAut.User != null ? chefeAut.Efectivo.Patente.Posto : "N/A"
                }
            ).FirstOrDefaultAsync();

            if (guia == null)
                return NotFound("Guia não encontrada.");

            // Buscar os rádios da guia
            var listaRadios = await _context.RadioMovimentos
                .Include(rm => rm.Radio)
                .Include(rm => rm.Radio.RadioTipo) // importante para acessar Marca/Modelo
                .Include(rm => rm.OrgUnidPnaMinint)
                .Where(rm => rm.RadioGuiaId == guia.Id && rm.TipoMovimento == "Saída")
                .Select(rm => new RadioItemViewModel
                {
                    Id = rm.Radio.Id,
                    CodRadio = rm.Radio.CodRadio,
                    Marca = rm.Radio.RadioTipo.Marca,
                    Modelo = rm.Radio.RadioTipo.Modelo,
                    IdRadio = rm.Radio.IdRadio,
                    TEI = rm.Radio.TEI,
                    NumSerie = rm.Radio.NumSerie,
                    EstadoTecnico = rm.Radio.EstadoTecnico,
                    TipoMovimento = rm.TipoMovimento,
                    OrgUnidPnaMinint = rm.OrgUnidPnaMinint != null
                        ? rm.OrgUnidPnaMinint.NomeOrgaoUnidade
                        : "N/A"
                })
                .ToListAsync();

            // Variáveis auxiliares
            int qtdRadio = listaRadios.Count;
            string OrgUnid = qtdRadio > 0 ? listaRadios[0].OrgUnidPnaMinint : "N/A";

            // Converter a lista de rádios em DataTable
            var dtRadios = new DataTable("Radios");
            dtRadios.Columns.Add("N", typeof(int)); // numeração sequencial
            dtRadios.Columns.Add("Id", typeof(int));
            dtRadios.Columns.Add("CodRadio", typeof(string));
            dtRadios.Columns.Add("Marca", typeof(string));
            dtRadios.Columns.Add("Modelo", typeof(string));
            dtRadios.Columns.Add("IdRadio", typeof(string));
            dtRadios.Columns.Add("TEI", typeof(string));
            dtRadios.Columns.Add("NumSerie", typeof(string));
            dtRadios.Columns.Add("EstadoTecnico", typeof(string));
            dtRadios.Columns.Add("TipoMovimento", typeof(string));
            dtRadios.Columns.Add("OrgUnidPnaMinint", typeof(string));

            // Preencher as linhas com numeração correta
            int index = 1;
            foreach (var r in listaRadios)
            {
                dtRadios.Rows.Add(
                    index++,
                    r.Id,
                    r.CodRadio,
                    r.Marca,
                    r.Modelo,
                    r.IdRadio,
                    r.TEI,
                    r.NumSerie,
                    r.EstadoTecnico,
                    r.TipoMovimento,
                    r.OrgUnidPnaMinint
                );
            }

            // Criar o relatório
            var report = new FastReport.Report();
            report.Load(caminhoReport);

            // Registrar DataTable como DataSource
            report.RegisterData(dtRadios, "Radios");
            report.GetDataSource("Radios").Enabled = true;

            // Registrar parâmetros da guia
            report.SetParameterValue("OrgUnid", OrgUnid.ToUpper());
            report.SetParameterValue("QtdRadio", qtdRadio);
            report.SetParameterValue("NumGuia", guia.NumGuia);
            report.SetParameterValue("NomeUsuario", guia.NomeUsuario);
            report.SetParameterValue("NomeChefe", guia.NomeChefe);
            report.SetParameterValue("PatenteChefe", guia.PatenteChefe.ToUpper());
            report.SetParameterValue("DataRegisto", guia.DataRegisto.ToString("dd/MM/yyyy HH:mm"));
            report.SetParameterValue("DataAprovacao", guia.DataAprovacao?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
            report.SetParameterValue("Aprovado", guia.Aprovado ? "Sim" : "Não");

            report.Prepare();

            // Exportar para PDF
            using var stream = new MemoryStream();
            var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
            report.Export(pdfExport, stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"GuiaRadio_{guia.NumGuia}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> VisualizarGuia(int id)
        {
            // Caminho do ficheiro FRX
            var caminhoReport = Path.Combine(_webHostEnv.WebRootPath, @"reports\RadioGuia.frx");

            if (!System.IO.File.Exists(caminhoReport))
                return NotFound("Modelo de relatório não encontrado. Crie-o primeiro.");

            // Buscar dados da Guia
            var guia = await (
                from g in _context.RadioGuias
                join userAut in _context.UsuarioAutes.Include(u => u.User)
                    on g.UserId equals userAut.UserId into criadorJoin
                from userAut in criadorJoin.DefaultIfEmpty()
                join chefeAut in _context.UsuarioAutes.Include(u => u.User)
                    on g.ChefeId equals chefeAut.UserId into chefeJoin
                from chefeAut in chefeJoin.DefaultIfEmpty()
                where g.Id == id
                select new
                {
                    g.Id,
                    g.NumGuia,
                    g.DataRegisto,
                    g.Aprovado,
                    g.DataAprovacao,
                    NomeUsuario = userAut != null && userAut.User != null ? userAut.User.UserName : "N/A",
                    NomeChefe = chefeAut != null && chefeAut.User != null ? chefeAut.Efectivo.NomeCompleto : "N/A",
                    PatenteChefe = chefeAut != null && chefeAut.User != null ? chefeAut.Efectivo.Patente.Posto : "N/A"
                }
            ).FirstOrDefaultAsync();

            if (guia == null)
                return NotFound("Guia não encontrada.");

            // Buscar rádios associados à guia
            var listaRadios = await _context.RadioMovimentos
                .Include(rm => rm.Radio)
                .Include(rm => rm.Radio.RadioTipo)
                .Include(rm => rm.OrgUnidPnaMinint)
                .Where(rm => rm.RadioGuiaId == guia.Id && rm.TipoMovimento == "Saída")
                .Select(rm => new RadioItemViewModel
                {
                    Id = rm.Radio.Id,
                    CodRadio = rm.Radio.CodRadio,
                    Marca = rm.Radio.RadioTipo.Marca,
                    Modelo = rm.Radio.RadioTipo.Modelo,
                    IdRadio = rm.Radio.IdRadio,
                    TEI = rm.Radio.TEI,
                    NumSerie = rm.Radio.NumSerie,
                    EstadoTecnico = rm.Radio.EstadoTecnico,
                    TipoMovimento = rm.TipoMovimento,
                    OrgUnidPnaMinint = rm.OrgUnidPnaMinint != null
                        ? rm.OrgUnidPnaMinint.NomeOrgaoUnidade
                        : "N/A"
                })
                .ToListAsync();

            if (!listaRadios.Any())
                return BadRequest("Nenhum rádio associado à guia.");

            // Dados auxiliares
            int qtdRadio = listaRadios.Count;
            string OrgUnid = listaRadios.FirstOrDefault()?.OrgUnidPnaMinint ?? "N/A";

            // Criar DataTable compatível com FastReport
            var dtRadios = new DataTable("Radios");
            dtRadios.Columns.Add("N", typeof(int));
            dtRadios.Columns.Add("Id", typeof(int));
            dtRadios.Columns.Add("CodRadio", typeof(string));
            dtRadios.Columns.Add("Marca", typeof(string));
            dtRadios.Columns.Add("Modelo", typeof(string));
            dtRadios.Columns.Add("IdRadio", typeof(string));
            dtRadios.Columns.Add("TEI", typeof(string));
            dtRadios.Columns.Add("NumSerie", typeof(string));
            dtRadios.Columns.Add("EstadoTecnico", typeof(string));
            dtRadios.Columns.Add("TipoMovimento", typeof(string));
            dtRadios.Columns.Add("OrgUnidPnaMinint", typeof(string));

            int index = 1;
            foreach (var r in listaRadios)
            {
                dtRadios.Rows.Add(
                    index++,
                    r.Id,
                    r.CodRadio,
                    r.Marca,
                    r.Modelo,
                    r.IdRadio,
                    r.TEI,
                    r.NumSerie,
                    r.EstadoTecnico,
                    r.TipoMovimento,
                    r.OrgUnidPnaMinint
                );
            }

            // Criar WebReport (para exibir no modal)
            var webReport = new WebReport();
            webReport.Report.Load(caminhoReport);

            // Registrar DataTable no relatório
            webReport.Report.RegisterData(dtRadios, "Radios");
            var ds = webReport.Report.GetDataSource("Radios");
            if (ds != null)
                ds.Enabled = true;

            // Passar parâmetros
            webReport.Report.SetParameterValue("OrgUnid", OrgUnid.ToUpper());
            webReport.Report.SetParameterValue("QtdRadio", qtdRadio);
            webReport.Report.SetParameterValue("NumGuia", guia.NumGuia);
            webReport.Report.SetParameterValue("NomeUsuario", guia.NomeUsuario);
            webReport.Report.SetParameterValue("NomeChefe", guia.NomeChefe);
            webReport.Report.SetParameterValue("PatenteChefe", guia.PatenteChefe.ToUpper());
            webReport.Report.SetParameterValue("DataRegisto", guia.DataRegisto.ToString("dd/MM/yyyy HH:mm"));
            webReport.Report.SetParameterValue("DataAprovacao", guia.DataAprovacao?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
            webReport.Report.SetParameterValue("Aprovado", guia.Aprovado ? "Sim" : "Não");

            // Opções do visualizador
            webReport.ShowToolbar = true;
            webReport.ShowRefreshButton = true;
            webReport.ShowPrint = true;

            // Retorna a partial com o relatório para o modal
            return PartialView("_VisualizarPDF", webReport);
        }

    }
}
