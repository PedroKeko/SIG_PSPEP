using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIG_PSPEP.Context;
using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Controllers
{
    [Area("Dpq")]
    public class EfectivosController : Controller
    {
        private readonly AppDbContext _context;

        public EfectivosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dpq/Efectivos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Efectivos.Include(e => e.FuncaoCargo).Include(e => e.OrgaoUnidade).Include(e => e.Patente).Include(e => e.SituacaoEfectivo).Include(e => e.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Dpq/Efectivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivo == null)
            {
                return NotFound();
            }

            return View(efectivo);
        }

        // GET: Dpq/Efectivos/Create
        public IActionResult Create()
        {
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo");
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade");
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto");
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SituacaoEfectivoId,OrgaoUnidadeId,FuncaoCargoId,PatenteId,Num_Processo,NIP,N_Agente,NomeCompleto,Apelido,Genero,DataNasc,EstadoCivil,GSanguineo,NumBI,BIValidade,BIEmitido,NumCartaConducao,CartaValidade,CartaEmitido,NumPassaporte,PassapValidade,PassapEmitido,Nacionalidade,Naturalidade,MunicipioRes,Destrito_BairroRes,Rua,CasaNum,Habilitacao,CursoHabilitado,InstitAcademica,Telefone1,Telefone2,Email,DataIngresso,TipoVinculo,Carreira,UnidadeOrigem,OutrasInfo,Id,Estado,DataRegisto,DataUltimaAlterecao,UserId")]
         Efectivo efectivo, IFormFile? FotoFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(efectivo);
                await _context.SaveChangesAsync(); // Primeiro, salvamos o Efectivo para gerar o ID

                if (FotoFile != null && FotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await FotoFile.CopyToAsync(memoryStream);
                        var fotoEfectivo = new FotoEfectivo
                        {
                            EfectivoId = efectivo.Id, // Relaciona com o efetivo recém-criado
                            Foto = memoryStream.ToArray()
                        };
                        _context.FotoEfectivos.Add(fotoEfectivo);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);

            return View(efectivo);
        }

        // GET: Dpq/Efectivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo == null)
            {
                return NotFound();
            }
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);
            return View(efectivo);
        }

        // POST: Dpq/Efectivos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SituacaoEfectivoId,OrgaoUnidadeId,FuncaoCargoId,PatenteId,Num_Processo,NIP,N_Agente,NomeCompleto,Apelido,Genero,DataNasc,EstadoCivil,GSanguineo,NumBI,BIValidade,BIEmitido,NumCartaConducao,CartaValidade,CartaEmitido,NumPassaporte,PassapValidade,PassapEmitido,Nacionalidade,Naturalidade,MunicipioRes,Destrito_BairroRes,Rua,CasaNum,Habilitacao,CursoHabilitado,InstitAcademica,Telefone1,Telefone2,Email,DataIngresso,TipoVinculo,Carreira,UnidadeOrigem,OutrasInfo,Id,Estado,DataRegisto,DataUltimaAlterecao,UserId")] Efectivo efectivo)
        {
            if (id != efectivo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(efectivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EfectivoExists(efectivo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FuncaoCargoId"] = new SelectList(_context.FuncaoCargos, "Id", "NomeFuncaoCargo", efectivo.FuncaoCargoId);
            ViewData["OrgaoUnidadeId"] = new SelectList(_context.OrgaoUnidades, "Id", "NomeOrgaoUnidade", efectivo.OrgaoUnidadeId);
            ViewData["PatenteId"] = new SelectList(_context.Patentes, "Id", "Posto", efectivo.PatenteId);
            ViewData["SituacaoEfectivoId"] = new SelectList(_context.SituacaoEfectivos, "Id", "TipoSituacao", efectivo.SituacaoEfectivoId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", efectivo.UserId);
            return View(efectivo);
        }

        // GET: Dpq/Efectivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var efectivo = await _context.Efectivos
                .Include(e => e.FuncaoCargo)
                .Include(e => e.OrgaoUnidade)
                .Include(e => e.Patente)
                .Include(e => e.SituacaoEfectivo)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (efectivo == null)
            {
                return NotFound();
            }

            return View(efectivo);
        }

        // POST: Dpq/Efectivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var efectivo = await _context.Efectivos.FindAsync(id);
            if (efectivo != null)
            {
                _context.Efectivos.Remove(efectivo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EfectivoExists(int id)
        {
            return _context.Efectivos.Any(e => e.Id == id);
        }
    }
}
