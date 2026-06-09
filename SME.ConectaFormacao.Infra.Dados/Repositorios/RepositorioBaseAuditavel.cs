using Dommel;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Linq.Expressions;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios;

public abstract class RepositorioBaseAuditavel<TEntidade>(
    IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : 
    IRepositorioBaseAuditavel<TEntidade>
    where TEntidade : EntidadeBaseAuditavel
{
    protected readonly IContextoAplicacao contexto = contexto;
    protected readonly IConectaFormacaoConexao conexao = conexao;

    public virtual Task<TEntidade> ObterPorId(long id)
        => conexao.Obter().GetAsync<TEntidade>(id);

    public async virtual Task<IList<TEntidade>> ObterTodos()
        => (await conexao.Obter().GetAllAsync<TEntidade>())
            .ToList();

    public virtual Task<TEntidade?> ObterPorExpressaoAsync(Expression<Func<TEntidade, bool>> predicado)
    {
        return conexao.Obter().FirstOrDefaultAsync(predicado);
    }

    public async virtual Task<IList<TEntidade>> ObterListaPorExpressaoAsync(Expression<Func<TEntidade, bool>> predicado)
    {
        var resultado = await conexao.Obter().SelectAsync(predicado);
        return [.. resultado];
    }

    public virtual async Task<long> Inserir(TEntidade entidade)
    {
        PreencherAuditoriaCriacao(entidade);
        entidade.Id = (long)await conexao.Obter().InsertAsync(entidade);
        return entidade.Id;
    }

    public async virtual Task<TEntidade> Atualizar(TEntidade entidade)
    {
        PreencherAuditoriaAlteracao(entidade);
        await conexao.Obter().UpdateAsync(entidade);
        return entidade;
    }

    public virtual async Task Remover(TEntidade entidade)
    {
        entidade.Excluido = true;
        await Atualizar(entidade);
    }

    public virtual async Task Remover(long id)
    {
        TEntidade entidade = await ObterPorId(id);
        await Remover(entidade);
    }

    protected void PreencherAuditoriaCriacao<T>(T entidade) where T : EntidadeBaseAuditavel
    {
        entidade.CriadoEm = DateTimeExtension.HorarioBrasilia();
        entidade.CriadoPor = contexto.NomeUsuario;
        entidade.CriadoLogin = contexto.UsuarioLogado;
    }

    protected void PreencherAuditoriaAlteracao<T>(T entidade) where T : EntidadeBaseAuditavel
    {
        entidade.AlteradoEm = DateTimeExtension.HorarioBrasilia();
        entidade.AlteradoPor = contexto.NomeUsuario;
        entidade.AlteradoLogin = contexto.UsuarioLogado;
    }

    public virtual async Task<TEntidade?> ObterNaoExcluidosPorIdAsync(long id)
    {
        return await conexao.Obter().FirstOrDefaultAsync<TEntidade>(e => e.Id == id && !e.Excluido);
    }

    public virtual async Task<IList<TEntidade>> ObterTodosNaoExcluidosAsync()
    {
        var resultado = await conexao.Obter().SelectAsync<TEntidade>(e => !e.Excluido);
        return resultado.ToList();
    }
}
