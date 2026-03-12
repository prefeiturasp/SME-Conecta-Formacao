namespace SME.ConectaFormacao.Infra.Dados.Templates
{
    public interface ITemplateService
    {
        string ObterTemplate(string nomeArquivo);
        string ObterImagemBase64(string nomeArquivoImagem);
        byte[] ObterTemplateBytes(string nomeArquivo);
    }
}
