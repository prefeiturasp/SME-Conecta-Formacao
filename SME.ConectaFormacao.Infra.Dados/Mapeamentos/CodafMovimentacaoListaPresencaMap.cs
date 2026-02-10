using Dapper.FluentMap.Dommel.Mapping;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafMovimentacaoListaPresencaMap : DommelEntityMap<CodafMovimentacaoListaPresenca>
    {
        public CodafMovimentacaoListaPresencaMap()
        {
            ToTable("codaf_movimentacao_lista_presenca");
            Map(c => c.Id).ToColumn("id").IsIdentity().IsKey();
            Map(c => c.CodafListaPresencaId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.StatusCodafListaPresenca).ToColumn("status_codaf_lista_presenca");
            Map(c => c.CodafComentarioListaPresencaId).ToColumn("codaf_comentario_lista_presenca_id");
            Map(c => c.CriadoEm).ToColumn("criado_em");
            Map(c => c.CriadoLogin).ToColumn("criado_login");
            Map(c => c.CriadoPor).ToColumn("criado_por");
        }
    }
}
