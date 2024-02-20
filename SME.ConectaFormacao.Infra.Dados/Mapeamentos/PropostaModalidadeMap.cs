using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaModalidadeMap : BaseMapAuditavel<PropostaModalidade>
    {
        public PropostaModalidadeMap()
        {
            ToTable("proposta_modalidade");

            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.Modalidade).ToColumn("modalidade");
        }
    }
}
