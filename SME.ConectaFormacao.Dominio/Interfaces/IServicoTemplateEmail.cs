namespace SME.ConectaFormacao.Dominio.Interfaces
{
    public interface IServicoTemplateEmail
    {
        Task<string> ObterHtmlInscricaoConfirmadaAsync(string nomeCursista, string nomeFormacao);
        Task<string> ObterHtmlInscricaoEmEsperaAsync(string nomeCursista, string nomeFormacao);
    }
}
