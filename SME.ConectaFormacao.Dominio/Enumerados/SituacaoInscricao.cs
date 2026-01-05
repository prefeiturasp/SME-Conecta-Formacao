using System.ComponentModel.DataAnnotations;

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
}