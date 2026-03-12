using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioRelatorios
    {
        Task<IEnumerable<InscritoFormacaoQueryModel>> ObterDadosRelatorioInscritosPorFormacaoAsync(
            FiltroRelatorioInscritosPorFormacaoDto filtro);
    }
}