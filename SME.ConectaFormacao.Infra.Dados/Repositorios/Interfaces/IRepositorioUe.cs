using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioUe
    {
        Task<ResultadoPaginado<AutocompletarNomeUeDto>> AutocompletarNomeAsync(string termo, long dreId, int numeroPagina, int numeroRegistros);
    }
}
