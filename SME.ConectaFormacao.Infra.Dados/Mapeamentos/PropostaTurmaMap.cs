using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTurmaMap : BaseMapAuditavel<PropostaTurma>
    {
        public PropostaTurmaMap()
        {
            ToTable("proposta_turma");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.Nome).ToColumn("nome");

            Map(c => c.Proposta).Ignore();
            Map(c => c.Dres).Ignore();
        }
    }
}
