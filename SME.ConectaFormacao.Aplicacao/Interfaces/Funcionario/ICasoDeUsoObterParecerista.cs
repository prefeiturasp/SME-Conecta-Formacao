using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Funcionario;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Funcionario
{
    public interface ICasoDeUsoObterParecerista
    {
        Task<IEnumerable<RetornoUsuarioLoginNomeDTO>> Executar();
    }
}