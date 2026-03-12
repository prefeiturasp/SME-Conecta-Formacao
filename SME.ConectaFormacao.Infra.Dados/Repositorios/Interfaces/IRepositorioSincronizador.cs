using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioSincronizador
    {
        Task SincronizarLoteCargosEolAsync(List<CargoEol> cargos, string codigoDre);
        Task LimparAtribuicaoServidorEolAsync(List<string> chavesExclusao);
        Task SincronizarLoteAtribuicaoServidorEolAsync(List<AtribuicaoServidorEol> atribuicoes);
        Task SincronizarLoteFuncaoAtividadeEolAsync(List<FuncaoAtividadeServidorEol> funcoesAtividade, string codigoDre);
        Task SincronizarLoteUeEolAsync(List<Ue> ues);
    }
}