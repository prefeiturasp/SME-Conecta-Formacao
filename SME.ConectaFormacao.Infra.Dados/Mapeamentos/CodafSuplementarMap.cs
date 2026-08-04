using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Mapeamentos
{
    public class CodafSuplementarMap : BaseMapAuditavel<CodafSuplementar>
    {
        public CodafSuplementarMap()
        {
            ToTable("codaf_suplementar");
            Map(c => c.CodafId).ToColumn("codaf_lista_presenca_id");
            Map(c => c.DataPublicacao).ToColumn("data_publicacao");
            Map(c => c.DataPublicacaoDom).ToColumn("data_publicacao_dom");
            Map(c => c.NumeroComunicado).ToColumn("numero_comunicado");
            Map(c => c.PaginaComunicadoDom).ToColumn("pagina_comunicado_dom");
            Map(c => c.CodigoCursoEol).ToColumn("codigo_curso_eol");
            Map(c => c.CodigoNivel).ToColumn("codigo_nivel");
            Map(c => c.Observacao).ToColumn("observacao");
            Map(c => c.Status).ToColumn("status");
            Map(c => c.CertificadoEmitido).Ignore();
            Map(c => c.CodafListaPresenca).Ignore();
            Map(c => c.CodafInscricoes).Ignore();
            Map(c => c.CodafRetificacoes).Ignore();
            Map(c => c.CodafAnexos).Ignore();
            Map(c => c.Proposta).Ignore();
            Map(c => c.PropostaTurma).Ignore();
        }
    }
}