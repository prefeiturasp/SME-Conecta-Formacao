using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaParecerMap : BaseMapAuditavel<PropostaParecer>
    {
        public PropostaParecerMap()
        {
            ToTable("proposta_parecer");

            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.Campo).ToColumn("campo");
            Map(t => t.Descricao).ToColumn("descricao");
            Map(t => t.Situacao).ToColumn("situacao");
        }
    }
}
