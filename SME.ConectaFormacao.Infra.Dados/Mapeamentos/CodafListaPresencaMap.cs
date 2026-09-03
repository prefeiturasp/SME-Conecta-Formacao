using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafListaPresencaMap : BaseMapAuditavel<CodafListaPresenca>
    {
        public CodafListaPresencaMap()
        {
            ToTable("codaf_lista_presenca");
            Map(c => c.PropostaTurmaId).ToColumn("proposta_turma_id");
            Map(c => c.PropostaId).ToColumn("proposta_id");
            Map(c => c.DataPublicacao).ToColumn("data_publicacao");
            Map(c => c.DataPublicacaoDom).ToColumn("data_publicacao_dom");
            Map(c => c.NumeroComunicado).ToColumn("numero_comunicado");
            Map(c => c.PaginaComunicadoDom).ToColumn("pagina_comunicado_dom");
            Map(c => c.CodigoCursoEol).ToColumn("codigo_curso_eol");
            Map(c => c.CodigoNivel).ToColumn("codigo_nivel");
            Map(c => c.Observacao).ToColumn("observacao");
            Map(c => c.Status).ToColumn("status");
            Map(c => c.CertificadoEmitido).Ignore();
            Map(c => c.Proposta).Ignore();
            Map(c => c.PropostaTurma).Ignore();
        }
    }
}