using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioFuncaoAtividadeUsuario
    {
        Task<IEnumerable<FuncaoAtividadeUsuario>> ObterPorRegistroFuncionalAsync(string cdRegistroFuncional);
        Task<DateTime?> ObterDataUltimaAtualizacaoAsync();
    }
}
