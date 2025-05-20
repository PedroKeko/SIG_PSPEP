using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dpq.Models
{
    public class PromocaoMultiplaDto
    {
        public List<EfectivoOrdemServico> Promocoes { get; set; } = new();
    }
}
