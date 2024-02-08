using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoProposta
    {
        [Display(Name = "Publicada",Prompt = "#297805")]
        Publicada = 1,

        [Display(Name = "Rascunho",Prompt = "#EEC25E")]
        Rascunho = 2,

        [Display(Name = "Cadastrada",Prompt = "#6464FF")]
        Cadastrada = 3,

        [Display(Name = "Aguardando análise do DF",Prompt = "#000000")]
        AguardandoAnaliseDf = 4,

        [Display(Name = "Aguardando análise da gestão",Prompt = "#000000")]
        AguardandoAnaliseGestao = 5,

        [Display(Name = "Desfavorável",Prompt = "#D06D12")]
        Desfavoravel = 6,

        [Display(Name = "Devolvida",Prompt = "#D06D12")]
        Devolvida = 7,
    }
    
}
