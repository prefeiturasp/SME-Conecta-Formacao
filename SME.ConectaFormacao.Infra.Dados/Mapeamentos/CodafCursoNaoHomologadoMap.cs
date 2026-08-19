using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafCursoNaoHomologadoMap : BaseMapAuditavel<CodafCursoNaoHomologado>
    {
        public CodafCursoNaoHomologadoMap()
        {
            ToTable("codaf_curso_nao_homologado");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.Observacao).ToColumn("observacao");
            Map(c => c.Status).ToColumn("status");
            Map(c => c.Proposta).Ignore();
            Map(c => c.PropostaTurma).Ignore();
            Map(c => c.DeclaracaoEmitida).Ignore();
        }
    }
}
