using SIG_PSPEP.Entidades;

namespace SIG_PSPEP.Areas.Dtti.Models
{
    public class RadioLocalizacaoViewModel
    {
        // Filtros
        public int? TipoId { get; set; }
        public int? OrgaoUnidadeId { get; set; }
        public string EstadoTecnico { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }

        // Listas de opções (para dropdowns)
        public List<RadioTipo> Tipos { get; set; } = new();
        public List<OrgUnidPnaMinint> OrgaosMinint { get; set; } = new();
        public List<string> EstadosTecnicos { get; set; } = new();

        // Resultado final
        public List<RadioMovimento> Radios { get; set; } = new();
    }
}
