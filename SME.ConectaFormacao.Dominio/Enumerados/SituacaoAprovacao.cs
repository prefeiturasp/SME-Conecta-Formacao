using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoAprovacao
    {
        [Display(Name = "Aprovado")]
        Aprovado = 1,
        [Display(Name = "Reprovado")]
        Reprovado = 2,
        [Display(Name = "Em andamento")]
        EmAndamento = 3
    }
}