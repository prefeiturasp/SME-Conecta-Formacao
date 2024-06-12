using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum NotificacaoTipoEnvio
    {
        [Display(Name = "E-mail")]
        Email = 1,

        [Display(Name = "SignalR")]
        SignalR = 2
    }
}