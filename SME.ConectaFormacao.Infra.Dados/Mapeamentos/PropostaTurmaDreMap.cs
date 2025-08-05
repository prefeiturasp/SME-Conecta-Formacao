using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class PropostaTurmaDreMap : BaseMapAuditavel<PropostaTurmaDre>
    {
        public PropostaTurmaDreMap()
        {
            ToTable("proposta_turma_dre");
            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.DreId).ToColumn("dre_id");

            Map(c => c.DreCodigo);
            Map(t => t.Dre).Ignore();
        }
    }
}
