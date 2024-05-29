using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao
{
    public class NotificacaoSignalRDTO
    {
        public long Id { get; set; }
        public DateTime DataHora { get; set; } = DateTimeExtension.HorarioBrasilia();
        public string Titulo { get; set; }
        public string[] Usuarios { get; set; }
    }
}