namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoObterNomeCursistaInscricao
    {
        Task<string> Executar(string? registroFuncional, string? cpf);
    }
}
