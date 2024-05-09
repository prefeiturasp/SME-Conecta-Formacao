using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoParecerista
    {
        [Display(Name = "Aprovada")]
        Aprovada = 1,

        [Display(Name = "Recusada")]
        Recusada = 2,

        [Display(Name = "Enviada")]
        Enviada = 3,

        [Display(Name = "Aguardando Validação")]
        AguardandoValidacao= 4,

        [Display(Name = "Adicionando revalidação")]
        AguardandoRevalidacao = 5,
        
        [Display(Name = "Desativado - Parecerista excluído")]
        Desativado = 7
    }

    public static class SituacaoPareceristaExtensao
    {
        public static bool EstaAprovada(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.Aprovada;
        }
        
        public static bool EstaRecusada(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.Recusada;
            
        }
        public static bool EstaEnviada(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.Enviada;
        }
        
        public static bool EstaAguardandoValidacao(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.AguardandoValidacao;
        }
        
        public static bool EstaAguardandoRevalidacao(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.AguardandoRevalidacao;
        }
        
        public static bool EstaDesativado(this SituacaoParecerista valor)
        {
            return valor == SituacaoParecerista.Desativado;
        }
    }
}