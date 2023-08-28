using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoIscricao
    {
        [Display(Name = "Optativa")]
        Optativa = 1,
        [Display(Name = "Automática")]
        Automatica = 2
    }
}
