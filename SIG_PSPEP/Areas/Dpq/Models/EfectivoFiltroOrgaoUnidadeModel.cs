using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class EfectivoFiltroOrgaoUnidadeModel
    {
        public int? OrgaoUnidadeId { get; set; }
        public DateTime? DataSelecionada { get; set; }
        public List<SelectListItem>? OrgaoUnidades { get; set; }
        public List<EfectivoSelecao>? Efectivos { get; set; }
    }
    public class EfectivoSelecao
    {
        public int Id { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Patente { get; set; }
        public string? NIP { get; set; }
        public string? OrgaoUnidade { get; set; }
        public bool Selecionado { get; set; }
    }
}
