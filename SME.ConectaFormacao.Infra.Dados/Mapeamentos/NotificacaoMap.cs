using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class NotificacaoMap : BaseMapAuditavel<Notificacao>
    {
        public NotificacaoMap()
        {
            ToTable("notificacao");
            Map(c => c.Titulo).ToColumn("titulo");
            Map(c => c.Mensagem).ToColumn("mensagem");
            Map(c => c.Categoria).ToColumn("categoria");
            Map(c => c.Tipo).ToColumn("tipo");
            Map(c => c.Parametros).ToColumn("parametros");
           
            Map(c => c.TipoEnvio).Ignore();
            Map(c => c.Usuarios).Ignore();
        }
    }
}
