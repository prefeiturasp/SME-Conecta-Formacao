using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario
{
    public interface ICasoDeUsoObterParecerista
    {
        Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar();
    }
}