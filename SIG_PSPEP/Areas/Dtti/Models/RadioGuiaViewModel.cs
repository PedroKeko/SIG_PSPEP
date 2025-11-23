namespace SIG_PSPEP.Areas.Dtti.Models
{
    public class RadioGuiaViewModel
    {
        public int Id { get; set; }
        public string? NumGuia { get; set; }
        public DateTime DataRegisto { get; set; }

        public string? NomeUsuario { get; set; }
        public string? NomeEfectivoUsuario { get; set; }

        public string? NomeChefe { get; set; }
        public string? NomeEfectivoChefe { get; set; }

        public int QuantidadeRadios { get; set; }
        public bool Aprovado { get; set; }
        public DateTime? DataAprovacao { get; set; }
    }

    public class RadioGuiaDetalhesViewModel
    {
        public int Id { get; set; }
        public string? NumGuia { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataRegisto { get; set; }
        public bool Aprovado { get; set; }
        public DateTime? DataAprovacao { get; set; }

        public string? NomeUsuario { get; set; }
        public string? NomeEfectivoUsuario { get; set; }
        public string? NomeChefe { get; set; }
        public string? NomeEfectivoChefe { get; set; }

        public List<RadioItemViewModel> Radios { get; set; } = new();
    }

    public class RadioItemViewModel
    {
        public int Id { get; set; }
        public string? CodRadio { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? IdRadio { get; set; }
        public string? NumSerie { get; set; }
        public string? TEI { get; set; }
        public string? EstadoTecnico { get; set; }
        public string? TipoMovimento { get; set; }
        public string? OrgUnidPnaMinint { get; set; }
    }
}
