using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum TipoEncontro
    {
        [Display(Name = "Presencial")]
        Presencial,
        [Display(Name = "Síncrono")]
        Sincrono,
        [Display(Name = "Assíncrono")]
        Assincrono
    }
}
