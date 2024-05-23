using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class Notificacao : EntidadeBaseAuditavel
    {
        public Notificacao()
        {
            Usuarios = Enumerable.Empty<NotificacaoUsuario>();
        }

        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public NotificacaoCategoria Categoria { get; set; }
        public NotificacaoTipo Tipo { get; set; }
        public NotificacaoTipoEnvio TipoEnvio { get; set; }
        public string Parametros { get; set; }
        public IEnumerable<NotificacaoUsuario> Usuarios { get; set; }
    }
}