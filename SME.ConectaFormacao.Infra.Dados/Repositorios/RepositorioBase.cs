using Dommel;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios;

public abstract class RepositorioBase<TEntidade> : IRepositorioBase<TEntidade>
    where TEntidade : EntidadeBase
{
    protected readonly IContextoAplicacao contexto;
    protected readonly IConectaFormacaoConexao conexao;

    public RepositorioBase(IContextoAplicacao contexto, IConectaFormacaoConexao conexao)
    {
        this.contexto = contexto;
        this.conexao = conexao;
    }

    public Task<TEntidade> ObterPorId(long id)
        => conexao.Obter().GetAsync<TEntidade>(id);

    public async Task<IList<TEntidade>> ObterTodos()
        => (await conexao.Obter().GetAllAsync<TEntidade>())
            .ToList();

    public async Task<long> Inserir(TEntidade entidade)
    {
        entidade.Id = (long)await conexao.Obter().InsertAsync(entidade);
        return entidade.Id;
    }

    public async Task<TEntidade> Atualizar(TEntidade entidade)
    {
        await conexao.Obter().UpdateAsync(entidade);
        return entidade;
    }

    public async Task Remover(TEntidade entidade)
    {
        entidade.Excluido = true;
        await Atualizar(entidade);
    }

    public async Task Remover(long id)
    {
        TEntidade entidade = await ObterPorId(id);
        await Remover(entidade);
    }
}
