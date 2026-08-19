using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafCursoNaoHomologadoInscricaoMap : BaseMapAuditavel<CodafCursoNaoHomologadoInscricao>
    {
        public CodafCursoNaoHomologadoInscricaoMap()
        {
            ToTable("codaf_curso_nao_homologado_inscricao");
            Map(c => c.CodafCursoNaoHomologadoId).ToColumn("codaf_curso_nao_homologado_id");
            Map(c => c.InscricaoId).ToColumn("inscricao_id");
            Map(c => c.Participou).ToColumn("participou");
            Map(c => c.CodafCursoNaoHomologado).Ignore();
            Map(c => c.Inscricao).Ignore();
        }
    }
}
