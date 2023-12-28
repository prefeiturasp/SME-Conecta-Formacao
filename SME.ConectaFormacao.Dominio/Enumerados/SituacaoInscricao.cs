using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoInscricao
    {
        [Display(Name ="Confirmada")]
        Confirmada = 1,
        [Display(Name = "Enviada")]
        Enviada = 2,
        [Display(Name = "Em Análise")]
        EmAnalise = 3,
        [Display(Name = "Cancelada")]
        Cancelada = 4
    }
}
