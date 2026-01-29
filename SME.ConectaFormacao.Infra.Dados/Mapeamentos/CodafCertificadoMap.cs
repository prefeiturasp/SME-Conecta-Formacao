using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafCertificadoMap : BaseMapAuditavel<CodafCertificado>
    {
        public CodafCertificadoMap()
        {
            ToTable("codaf_certificados");

            Map(p => p.CodigoCertificado)
                .ToColumn("codigo_certificado")
                .IsIdentity();

            Map(p => p.CodafInscricaoListaPresencaId).ToColumn("codaf_inscricao_lista_presenca_id");
            Map(p => p.PropostaRegenteTurmaId).ToColumn("proposta_regente_turma_id");
            Map(p => p.TipoParticipacao).ToColumn("tipo_participacao");
            Map(p => p.DataEmissao).ToColumn("data_emissao");
            Map(p => p.HtmlContentSnapshot).ToColumn("html_content_snapshot");
            Map(p => p.MetadadosJson).ToColumn("metadados_json");
        }
    }
}
