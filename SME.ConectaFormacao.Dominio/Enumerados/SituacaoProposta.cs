using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoProposta
    {
        [Display(Name = "Favorável")]
        Favoravel = 1,

        [Display(Name = "Rascunho")]
        Rascunho = 2,

        [Display(Name = "Cadastrada")]
        Cadastrada = 3,

        [Display(Name = "Aguardando análise do DF")]
        AguardandoAnaliseDf = 4,
        
        [Display(Name = "Aguardando análise da gestão")]
        AguardandoAnaliseGestao = 5,
        
        [Display(Name = "Desfavorável")]
        Desfavoravel = 6,
        
        [Display(Name = "Devolvida")]
        Devolvida = 7,
    }
}
