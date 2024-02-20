using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTutorTurmaMap : BaseMapAuditavel<PropostaTutorTurma>
    {
        public PropostaTutorTurmaMap()
        {
            ToTable("proposta_tutor_turma");
            Map(t => t.PropostaTutorId).ToColumn("proposta_tutor_id");
            Map(t => t.TurmaId).ToColumn("turma_id");
        }
    }
}