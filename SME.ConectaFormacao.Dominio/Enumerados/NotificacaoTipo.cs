using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum NotificacaoTipo
    {
        [Display(Name = "Proposta")]
        Proposta = 1,
        [Display(Name = "Codaf")]
        Codaf = 2
    }
}