namespace SME.ConectaFormacao.Dominio.Repositorios;

public interface IRepositorioBaseAuditavel<TEntidade> 
    where TEntidade : EntidadeBaseAuditavel    
{
    Task<TEntidade> ObterPorId(long id);
    Task<IList<TEntidade>> ObterTodos();
    Task<long> Inserir(TEntidade entidade);
    Task<TEntidade> Atualizar(TEntidade entidade);
    Task Remover(TEntidade entidade);
    Task Remover(long id);
}
