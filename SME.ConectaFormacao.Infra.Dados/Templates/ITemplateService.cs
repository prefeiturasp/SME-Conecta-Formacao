namespace SME.ConectaFormacao.Infra.Dados.Templates
{
    public interface ITemplateService
    {
        string ObterTemplateCertificado(string nomeArquivo);
        string ObterImagemBase64(string nomeArquivoImagem);
    }
}
