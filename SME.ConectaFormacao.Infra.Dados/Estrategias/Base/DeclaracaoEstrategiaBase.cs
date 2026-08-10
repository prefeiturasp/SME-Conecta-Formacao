using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Base
{
    public abstract class DeclaracaoEstrategiaBase(ITemplateService templateService)
    {
        protected ITemplateService templateService = templateService;
        
        // Constantes com os caminhos EXATOS conforme definido no .csproj
        private const string TEMPLATE_LAYOUT = "SME.ConectaFormacao.Infra.Dados.Templates.layout-declaracao-codaf.html";
        private const string IMAGEM_HEADER = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.header.png";
        private const string IMAGEM_BRASAO = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.brasao.png";
        private const string IMAGEM_SELO = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.selo.svg";
        private const string IMAGEM_ASSINATURA = "SME.ConectaFormacao.Infra.Dados.Templates.Assets.assinatura.png";

        protected string ObterLayoutBase(DadosEmissaoDeclaracaoCodafDto dados)
        {
            var layout = templateService.ObterTemplate(TEMPLATE_LAYOUT);

            var emissorRodape = ObterTextoEmissorRodape(dados);
            var dataEmissao = DateTime.Now.ToString("dd/MM/yyyy");
            var numHomologacao = dados.NumeroHomologacao?.ToString() ?? "N/A";
            var numCodigoDeclaracao = dados.NumeroCodigoDeclaracao?.ToString() ?? "N/A";

            var imgHeader = templateService.ObterImagemBase64(IMAGEM_HEADER);
            var brasao = templateService.ObterImagemBase64(IMAGEM_BRASAO);
            var selo = templateService.ObterImagemBase64(IMAGEM_SELO);
            var assinatura = templateService.ObterImagemBase64(IMAGEM_ASSINATURA);

            return layout
                .Replace("{{EMISSOR}}", emissorRodape)
                .Replace("{{NUM_CODIGO_DECLARACAO}}", numCodigoDeclaracao)
                .Replace("{{NUM_HOM_FORMACAO}}", numHomologacao)
                .Replace("{{HEADER}}", imgHeader)
                .Replace("{{BRASAO}}", brasao)
                .Replace("{{SELO}}", selo)
                .Replace("{{ASSINATURA}}", assinatura)
                .Replace("{{DATA_EMISSAO}}", dataEmissao);
        }

        protected static string ObterTextoEmissorCorpo(DadosEmissaoDeclaracaoCodafDto dados)
        {
            if (dados.TipoEmissor == TipoEmissor.Coordenadoria)
                return string.IsNullOrWhiteSpace(dados.EmissorSigla)
                    ? dados.Emissor
                    : $"{dados.EmissorSigla} - {dados.Emissor}";

            return dados.Emissor;
        }

        protected static string? ObterTextoEmissorRodape(DadosEmissaoDeclaracaoCodafDto dados)
        {
            return dados.TipoEmissor == TipoEmissor.Coordenadoria
                ? dados.EmissorSigla
                : dados.Emissor;
        }
    }
}