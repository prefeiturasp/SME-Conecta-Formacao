using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoEmissor
    {
        [Display(Name = "DRE")]
        Dre = 1,
        [Display(Name = "Coordenadoria")]
        Coordenadoria = 2
    }
}
