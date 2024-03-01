using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class ComponenteCurricularMap : BaseMapAuditavel<ComponenteCurricular>
    {
        public ComponenteCurricularMap()
        {
            ToTable("componente_curricular");

            Map(c => c.AnoTurmaId).ToColumn("ano_turma_id");
            Map(c => c.CodigoEOL).ToColumn("codigo_eol");
            Map(c => c.Nome).ToColumn("nome");
            Map(c => c.Todos).ToColumn("todos");
            Map(c => c.Ordem).ToColumn("ordem");

            Map(c => c.AnoTurma).Ignore();
        }
    }
}
