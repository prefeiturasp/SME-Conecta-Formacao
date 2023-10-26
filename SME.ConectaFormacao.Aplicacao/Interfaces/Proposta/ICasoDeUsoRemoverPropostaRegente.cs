namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRemoverPropostaRegente
    {
        Task<bool> Executar(long regenteId);
    }
}