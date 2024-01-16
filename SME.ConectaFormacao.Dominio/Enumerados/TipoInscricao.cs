using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoInscricao
    {
        [Display(Name = "Optativa")]
        Optativa = 1,
        [Display(Name = "Automática")]
        Automatica = 2,
        [Display(Name = "Automática (JEIF)")]
        AutomaticaJEIF = 3
    }
}
