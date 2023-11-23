using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaMovimentacaoMap : BaseMapAuditavel<PropostaMovimentacao>
    {
        public PropostaMovimentacaoMap()
        {
            ToTable("proposta_movimentacao");

            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.Justificativa).ToColumn("justificativa");
            Map(c => c.Situacao).ToColumn("situacao");
        }
    }
}
