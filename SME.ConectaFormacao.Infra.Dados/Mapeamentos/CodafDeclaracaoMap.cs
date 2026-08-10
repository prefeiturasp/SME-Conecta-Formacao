using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafDeclaracaoMap : BaseMapAuditavel<CodafDeclaracao>
    {
        public CodafDeclaracaoMap()
        {
            ToTable("codaf_declaracoes");
            Map(p => p.CodigoDeclaracao)
                .ToColumn("codigo_declaracao")
                .IsIdentity();

            Map(p => p.CodafCursoNaoHomologadoInscricaoId).ToColumn("codaf_curso_nao_homologado_inscricao_id");
            Map(p => p.PropostaRegenteTurmaId).ToColumn("proposta_regente_turma_id");
            Map(p => p.TipoParticipacao).ToColumn("tipo_participacao");
            Map(p => p.DataEmissao).ToColumn("data_emissao");
            Map(p => p.HtmlContentSnapshot).ToColumn("html_content_snapshot");
            Map(p => p.MetadadosJson).ToColumn("metadados_json");
            Map(p => p.ChaveObjetoArmazenamento).ToColumn("chave_objeto_armazenamento");
            Map(p => p.ErroProcessamento).ToColumn("erro_processamento");
            Map(p => p.StatusProcessamento).ToColumn("status_processamento");
            Map(p => p.CodafCursoNaoHomologadoId).ToColumn("codaf_curso_nao_homologado_id");
        }
    }
}
