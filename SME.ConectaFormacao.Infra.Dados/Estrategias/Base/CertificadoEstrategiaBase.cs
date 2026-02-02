using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Base
{
    public abstract class CertificadoEstrategiaBase(ITemplateService templateService)
    {
        protected ITemplateService templateService = templateService;
        protected string ObterLayoutBase(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = templateService.ObterTemplateCertificado("layout-certificado-codaf.html");
            var imgBrasaoTituloSme = templateService.ObterImagemBase64("brasao_sme.png");
            var imgAssinaturaSecretario = templateService.ObterImagemBase64("assinatura_secretario.png");
            var imgBrasaoPrefeitura = templateService.ObterImagemBase64("brasao_prefeitura.png");
            var dataAtualAssinatura = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
            var anoAtual = DateTime.Now.ToString("yyyy");
            var numComunicado = dados.NumeroComunicado.ToString();
            var dataPublicacao = dados.DataPublicacao.ToString("dd/MM/yyyy");
            var numHomologacao = dados.NumeroHomologacao?.ToString() ?? "N/A";
            return layout
                .Replace("{{IMG_BRASAO_TITULO_SME}}", imgBrasaoTituloSme)
                .Replace("{{IMG_BRASAO_PREFEITURA}}", imgBrasaoPrefeitura)
                .Replace("{{IMG_ASSINATURA_SECRETARIO}}", imgAssinaturaSecretario)
                .Replace("{{DATA_ATUAL_ASSINATURA}}", dataAtualAssinatura)
                .Replace("{{ANO_ATUAL}}", anoAtual)
                .Replace("{{NUM_COMUNICADO}}", numComunicado)
                .Replace("{{DATA_PUBLICACAO_CODAF}}", dataPublicacao)
                .Replace("{{NUM_HOM_FORMACAO}}", numHomologacao);
        }
    }
}