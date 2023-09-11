using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoProposta
    {
        [Display(Name = "Ativo")]
        Ativo = 1,
        [Display(Name = "Rascunho")]
        Rascunho = 2
    }
}
