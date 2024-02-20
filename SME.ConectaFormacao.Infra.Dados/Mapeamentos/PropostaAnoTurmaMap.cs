using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaAnoTurmaMap : BaseMapAuditavel<PropostaAnoTurma>
    {
        public PropostaAnoTurmaMap()
        {
            ToTable("proposta_ano_turma");

            Map(t => t.PropostaId).ToColumn("proposta_id");
            Map(t => t.AnoTurmaId).ToColumn("ano_turma_id");
        }
    }
}
