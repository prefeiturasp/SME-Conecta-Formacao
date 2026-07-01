using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Base
{
    public abstract class CertificadoEstrategiaBase(ITemplateService templateService)
    {
        protected ITemplateService templateService = templateService;
        
        // Constantes com os caminhos EXATOS conforme definido no .csproj
        private const string TEMPLATE_LAYOUT = "SME.ConectaFormacao.Infra.Dados.Templates.layout-certificado-codaf.html";
        private const string IMAGEM_HEADER = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.header.png";
        private const string IMAGEM_BRASAO = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.brasao.png";
        private const string IMAGEM_SELO = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.selo.svg";
        private const string IMAGEM_ASSINATURA = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.assinatura.png";

        protected string ObterLayoutBase(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = templateService.ObterTemplate(TEMPLATE_LAYOUT);
            
            var emissor = dados.Emissor;
            var numComunicado = dados.NumeroComunicado.ToString();
            var dataPublicacao = dados.DataPublicacao.ToString("dd/MM/yyyy");
            var paginaDiarioOficial = dados.PaginaDiarioOficial.ToString();
            var numHomologacao = dados.NumeroHomologacao?.ToString() ?? "N/A";
            var dataEmissao = DateTime.Now.ToString("dd/MM/yyyy");

            var imgHeader = templateService.ObterImagemBase64(IMAGEM_HEADER);
            var brasao = templateService.ObterImagemBase64(IMAGEM_BRASAO);
            var selo = templateService.ObterImagemBase64(IMAGEM_SELO);
            var assinatura = templateService.ObterImagemBase64(IMAGEM_ASSINATURA);

            return layout
                .Replace("{{EMISSOR}}", emissor)
                .Replace("{{NUM_COMUNICADO}}", numComunicado)
                .Replace("{{DATA_PUBLICACAO_CODAF}}", dataPublicacao)
                .Replace("{{PAG_DIARIO_OFICIAL}}", paginaDiarioOficial)
                .Replace("{{NUM_HOM_FORMACAO}}", numHomologacao)
                .Replace("{{HEADER}}", imgHeader)
                .Replace("{{BRASAO}}", brasao)
                .Replace("{{SELO}}", selo)
                .Replace("{{ASSINATURA}}", assinatura)
                .Replace("{{DATA_EMISSAO}}", dataEmissao);
        }
    }
}