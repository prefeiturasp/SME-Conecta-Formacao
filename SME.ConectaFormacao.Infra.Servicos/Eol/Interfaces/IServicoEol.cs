namespace SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces
{
    public interface IServicoEol
    {
        Task<string> ObterNomeProfissionalPorRegistroFuncional(string registroFuncional);
    }
}