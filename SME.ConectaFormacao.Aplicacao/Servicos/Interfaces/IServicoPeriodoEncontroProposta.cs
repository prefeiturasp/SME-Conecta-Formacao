namespace SME.ConectaFormacao.Aplicacao.Servicos.Interfaces
{
    public interface IServicoPeriodoEncontroProposta
    {
        Task<string> ObterPeriodoEncontrosTurmaAsync(long turmaId);
    }
}
