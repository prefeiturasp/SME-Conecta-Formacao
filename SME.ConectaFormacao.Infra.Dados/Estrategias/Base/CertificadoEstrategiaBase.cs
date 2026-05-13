using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Base
{
    public abstract class CertificadoEstrategiaBase(ITemplateService templateService)
    {
        protected ITemplateService templateService = templateService;
        protected string ObterLayoutBase(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = templateService.ObterTemplate("layout-certificado-codaf.html");
            var coordenadoriaOuDre = dados.DreCoordenadoria;
            var numCodigoCertificado = dados.CodigoCertificado.ToString();
            var numComunicado = dados.NumeroComunicado.ToString();
            var dataPublicacao = dados.DataPublicacao.ToString("dd/MM/yyyy");
            var paginaDiarioOficial = dados.PaginaDiarioOficial.ToString();
            var numHomologacao = dados.NumeroHomologacao?.ToString() ?? "N/A";
            return layout
                .Replace("{{COORDENADORIA_OU_DRE}}", coordenadoriaOuDre)
                .Replace("{{NUM_CODIGO_CERTIFICADO}}", numCodigoCertificado)
                .Replace("{{NUM_COMUNICADO}}", numComunicado)
                .Replace("{{DATA_PUBLICACAO_CODAF}}", dataPublicacao)
                .Replace("{{PAG_DIARIO_OFICIAL}}", paginaDiarioOficial)
                .Replace("{{NUM_HOM_FORMACAO}}", numHomologacao);
        }
    }
}