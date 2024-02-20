namespace SME.ConectaFormacao.Aplicacao.Interfaces.Proposta
{
    public interface ICasoDeUsoRemoverPropostaEncontro
    {
        Task<bool> Executar(long id);
    }
}
