namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoAlterarVinculoInscricao
    {
        Task<bool> Executar(long id, int tipoVinculo);
    }
}