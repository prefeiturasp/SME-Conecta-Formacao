using SME.ConectaFormacao.Dominio.Extensoes;
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

    public static class TipoInscricaoExtensao
    {
        public static bool EhAutomaticaOuJEIF(this TipoInscricao? valor)
        {
            if (valor.EhNulo())
                return default;

            return (valor == TipoInscricao.Automatica || valor == TipoInscricao.AutomaticaJEIF);
        }

        public static bool EhAutomaticaOuJEIF(this TipoInscricao valor)
        {
            return (valor == TipoInscricao.Automatica || valor == TipoInscricao.AutomaticaJEIF);
        }
    }
}
