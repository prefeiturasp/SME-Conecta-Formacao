using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafCursoNaoHomologadoAnexoMap : BaseMapAuditavel<CodafCursoNaoHomologadoAnexo>
    {
        public CodafCursoNaoHomologadoAnexoMap()
        {
            ToTable("codaf_curso_nao_homologado_anexo");
            Map(c => c.CodafCursoNaoHomologadoId).ToColumn("codaf_curso_nao_hom_id");
            Map(c => c.ArquivoCodigo).ToColumn("arquivo_codigo");
            Map(c => c.NomeArquivo).ToColumn("nome_arquivo");
            Map(c => c.Extensao).ToColumn("extensao");
            Map(c => c.TipoAnexoId).ToColumn("tipo_anexo_id");
            Map(c => c.CodafCursoNaoHomologado).Ignore();
        }
    }
}
