using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoProposta
    {
        [Display(Name = "Publicada", Prompt = "#297805")]
        Publicada = 1,

        [Display(Name = "Rascunho", Prompt = "#EEC25E")]
        Rascunho = 2,

        [Display(Name = "Cadastrada", Prompt = "#6464FF")]
        Cadastrada = 3,

        [Display(Name = "Aguardando análise do DF", Prompt = "#000000")]
        AguardandoAnaliseDf = 4,

        [Display(Name = "Aguardando análise da gestão", Prompt = "#000000")]
        AguardandoAnaliseGestao = 5,

        [Display(Name = "Desfavorável", Prompt = "#D06D12")]
        Desfavoravel = 6,

        [Display(Name = "Devolvida", Prompt = "#D06D12")]
        Devolvida = 7,

        [Display(Name = "Alterando", Prompt = "#297805")]
        Alterando = 8,

        [Display(Name = "Aprovada", Prompt = "#008000")]
        Aprovada = 9,

        [Display(Name = "Aguardando análise do Parecerista", Prompt = "#000000")]
        AguardandoAnaliseParecerista = 10,
        
        [Display(Name = "Análise do parecer pela área promotora", Prompt = "#000000")]
        AnaliseParecerAreaPromotora = 11
    }

    public static class SituacaoPropostaExtensao
    {
        public static bool EstaPublicada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Publicada;
        }

        public static bool EstaCadastrada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Cadastrada;
        }

        public static bool EhRascunho(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Rascunho;
        }

        public static bool EstaAguardandoAnaliseDf(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnaliseDf;
        }

        public static bool EstaAguardandoAnaliseGestao(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.AguardandoAnaliseGestao;
        }

        public static bool EstaDesfavoravel(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Desfavoravel;
        }

        public static bool EstaDevolvida(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Devolvida;
        }

        public static bool EhParaSalvarRascunho(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Rascunho || valor == SituacaoProposta.Alterando;
        }

        public static bool EhAlterando(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Alterando;
        }

        public static bool EhAprovada(this SituacaoProposta valor)
        {
            return valor == SituacaoProposta.Aprovada;
        }
    }
}
