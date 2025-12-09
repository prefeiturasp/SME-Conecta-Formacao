using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioFuncaoAtividadeUsuario
    {
        Task<IEnumerable<FuncaoAtividadeServidorEol>> ObterPorRegistroFuncionalAsync(string cdRegistroFuncional);
        Task<DateTime?> ObterDataUltimaAtualizacaoAsync(string codigoDre);
    }
}
