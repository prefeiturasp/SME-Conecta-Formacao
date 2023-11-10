namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoEnviarPropostaParaDf
    {
        Task Executar(long propostaId);
    }
}