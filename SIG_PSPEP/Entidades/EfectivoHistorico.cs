namespace SIG_PSPEP.Entidades
{
    public class EfectivoHistorico
    {
        public int Id { get; set; }
        public int EfectivoId { get; set; }
        public string? Tipo { get; set; } // "Pedido", "Promoção", "Nomeação"
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }
        public string? Status { get; set; } // "Aprovado", "Pendente", "Rejeitado"
        public Efectivo? Efectivo { get; set; }
    }
}
