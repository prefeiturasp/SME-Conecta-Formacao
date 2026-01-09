using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Notificacao : EntidadeBaseAuditavel
    {
        public Guid? CorrelacaoId { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensagem { get; set; } = null!;
        public NotificacaoCategoria Categoria { get; set; }
        public NotificacaoTipo Tipo { get; set; }
        public NotificacaoTipoOrigem? TipoOrigem { get; set; }
        public NotificacaoTipoEnvio? TipoEnvio { get; set; }
        public string Parametros { get; set; } = null!;
        public IEnumerable<NotificacaoUsuario> Usuarios { get; set; } = [];
    }
}