namespace SME.ConectaFormacao.Dominio.Entidades;

public interface IDominioBase<TEntidade>
    where TEntidade : EntidadeBase
{
    Task<TEntidade> ObterPorId(long id);
    Task<IEnumerable<TEntidade>> ObterTodos();
    Task<long> Inserir(TEntidade entidade);
    Task<TEntidade> Atualizar(TEntidade entidade);
}
