namespace SME.ConectaFormacao.Dominio.Entidades;

public interface IDominioBaseAuditavel<TEntidade>
    where TEntidade : EntidadeBaseAuditavel
{
    Task<TEntidade> ObterPorId(long id);
    Task<IList<TEntidade>> ObterTodos();
    Task<long> Inserir(TEntidade entidade);
    Task<TEntidade> Atualizar(TEntidade entidade);
}
