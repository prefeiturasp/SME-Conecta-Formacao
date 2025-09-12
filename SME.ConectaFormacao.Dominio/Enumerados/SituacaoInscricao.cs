using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum SituacaoInscricao
    {
        [Display(Name = "Confirmada")]
        Confirmada = 1,
        [Display(Name = "Enviada")]
        Enviada = 2,
        [Display(Name = "Aguardando Análise")]
        AguardandoAnalise = 3,
        [Display(Name = "Cancelada")]
        Cancelada = 4,
        [Display(Name = "Em Espera")]
        EmEspera = 5,
        [Display(Name = "Transferida")]
        Transferida = 6
    }

    public static class SituacaoInscricaoExtensao
    {
        public static bool EhConfirmada(this SituacaoInscricao situacao)
        {
            return situacao == SituacaoInscricao.Confirmada;
        }

        public static bool EhEnviada(this SituacaoInscricao situacao)
        {
            return situacao == SituacaoInscricao.Enviada;
        }

        public static bool EhAguardandoAnalise(this SituacaoInscricao situacao)
        {
            return situacao == SituacaoInscricao.AguardandoAnalise;
        }

        public static bool NaoEhAguardandoAnalise(this SituacaoInscricao situacao)
        {
            return situacao != SituacaoInscricao.AguardandoAnalise;
        }

        public static bool NaoEhAguardandoAnaliseEEmEspera(this SituacaoInscricao situacao)
        {
            return situacao != SituacaoInscricao.AguardandoAnalise && situacao != SituacaoInscricao.EmEspera;
        }

        public static bool EhCancelada(this SituacaoInscricao situacao)
        {
            return situacao == SituacaoInscricao.Cancelada;
        }

        public static bool NaoEhCancelada(this SituacaoInscricao situacao)
        {
            return situacao != SituacaoInscricao.Cancelada;
        }

        public static bool EhEmEspera(this SituacaoInscricao situacao)
        {
            return situacao == SituacaoInscricao.EmEspera;
        }
    }
}
