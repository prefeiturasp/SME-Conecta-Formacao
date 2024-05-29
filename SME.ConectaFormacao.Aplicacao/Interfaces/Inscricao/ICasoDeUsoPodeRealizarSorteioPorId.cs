namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao
{
    public interface ICasoDeUsoPodeRealizarSorteioPorId
    {
        Task<bool> Executar(long propostaId);
    }
}