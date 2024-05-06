using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum OrigemInscricao
    {
        [Display(Name = "Automática")]
        Automatica = 1,
        [Display(Name = "Manual")]
        Manual = 2,
    }
}
