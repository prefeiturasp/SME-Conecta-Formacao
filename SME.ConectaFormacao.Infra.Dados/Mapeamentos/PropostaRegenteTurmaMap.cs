using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaRegenteTurmaMap : BaseMapAuditavel<PropostaRegenteTurma>
    {
        public PropostaRegenteTurmaMap()
        {
            ToTable("proposta_regente_turma");
            Map(t => t.PropostaRegenteId).ToColumn("proposta_regente_id");
            Map(t => t.TurmaId).ToColumn("turma_id");

            Map(t => t.Turma).Ignore();
        }
    }
}