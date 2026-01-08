using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafComentarioListaPresencaMap : BaseMapAuditavel<CodafComentarioListaPresenca>
    {
        public CodafComentarioListaPresencaMap()
        {
            ToTable("codaf_comentario_lista_presenca");
            Map(c => c.CodafListaPresencaId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.Comentario).ToColumn("comentario");
            Map(c => c.NotificacaoEnviada).ToColumn("notificacao_enviada");
            Map(c => c.DataNotificacao).ToColumn("data_notificacao");
            Map(c => c.Ativo).ToColumn("ativo");
            Map(c => c.CodafListaPresenca).Ignore();
        }
    }
}
