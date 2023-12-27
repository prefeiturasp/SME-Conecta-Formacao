using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTurmaVagaMap : BaseMapAuditavel<PropostaTurmaVaga>
    {
        public PropostaTurmaVagaMap()
        {
            ToTable("proposta_turma_vaga");

            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.InscricaoId).ToColumn("inscricao_id");
        }
    }
}
