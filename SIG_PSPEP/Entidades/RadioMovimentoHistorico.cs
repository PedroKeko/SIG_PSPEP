using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIG_PSPEP.Entidades
{
    public class RadioMovimentoHistorico
    {
        [Key]
        public int Id { get; set; }

        public int RadioMovimentoId { get; set; }
        public RadioMovimento? RadioMovimento { get; set; }

        [DisplayName("Data do Evento")]
        public DateTime DataEvento { get; set; } = DateTime.Now;

        [DisplayName("Tipo de Alteração")]
        public string? TipoHistorico { get; set; } // Criado, Atualizado, Aprovado, Rejeitado, etc.

        [DisplayName("Responsável")]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [DisplayName("Descrição")]
        public string? Descricao { get; set; }
    }

}
