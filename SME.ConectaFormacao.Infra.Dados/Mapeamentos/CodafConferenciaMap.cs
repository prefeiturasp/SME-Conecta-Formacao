using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    [ExcludeFromCodeCoverage]
    public class CodafConferenciaMap : BaseMapAuditavel<CodafConferencia>
    {
        public CodafConferenciaMap()
        {
            ToTable("codaf_conferencia");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.NumeroComunicado).ToColumn("numero_comunicado");
            Map(c => c.DataComunicado).ToColumn("data_comunicado");
            Map(c => c.PaginaComunicadoDom).ToColumn("pagina_comunicado_dom");
            Map(c => c.DataPublicacaoDom).ToColumn("data_publicacao_dom");
            Map(c => c.CodigoCursoEol).ToColumn("codigo_curso_eol");
            Map(c => c.CodigoNivel).ToColumn("codigo_nivel");
            Map(c => c.Observacao).ToColumn("observacao");
            Map(c => c.Status).ToColumn("status");
            Map(c => c.DataFinalizacao).ToColumn("data_finalizacao");
            Map(c => c.CertificadoEmitido).Ignore();
            Map(c => c.Proposta).Ignore();
            Map(c => c.PropostaTurma).Ignore();
            Map(c => c.CodafInscricoes).Ignore();
            Map(c => c.CodafAnexos).Ignore();
            Map(c => c.CodafCertificados).Ignore();
        }
    }
}
