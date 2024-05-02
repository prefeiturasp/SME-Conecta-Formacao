using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoParecer
    {
        [Display(Name = "Pendente de envio do parecer pelo parecerista")]
        PendenteEnvioParecerPeloParecerista = 1,
        
        [Display(Name = "Aguardando análise do parecer pelo Admin DF")]
        AguardandoAnaliseParecerPeloAdminDF = 2,
        
        [Display(Name = "Aguardando análise do parecer pela área promotora")]
        AguardandoAnaliseParecerPelaAreaPromotora = 3
    }

    public static class SituacaoParecerExtensao
    {
        public static bool EstaPendenteEnvioParecerPeloParecerista(this SituacaoParecer valor)
        {
            return valor == SituacaoParecer.PendenteEnvioParecerPeloParecerista;
        }
        
        public static bool EstaAguardandoAnaliseParecerPeloAdminDF(this SituacaoParecer valor)
        {
            return valor == SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF;
        }
        
        public static bool EstaAguardandoAnaliseParecerPelaAreaPromotora(this SituacaoParecer valor)
        {
            return valor == SituacaoParecer.AguardandoAnaliseParecerPelaAreaPromotora;
        }
        
        public static bool EstaAguardandoAnaliseParecerPeloDfOuPelaAreaPromotora(this SituacaoParecer valor)
        {
            return valor == SituacaoParecer.AguardandoAnaliseParecerPelaAreaPromotora || valor == SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF;
        }
    }
}