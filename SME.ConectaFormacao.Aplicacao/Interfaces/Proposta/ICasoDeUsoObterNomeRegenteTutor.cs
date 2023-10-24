namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoObterNomeRegenteTutor
    {
        Task<string> Executar(string registroFuncional);
    }
}