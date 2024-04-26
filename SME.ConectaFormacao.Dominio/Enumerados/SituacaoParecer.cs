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
}