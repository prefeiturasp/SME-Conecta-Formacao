using Dommel;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Repositorios;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios;

public abstract class RepositorioBaseAuditavel<TEntidade> : IRepositorioBaseAuditavel<TEntidade>
    where TEntidade : EntidadeBaseAuditavel
{
    protected readonly IContextoAplicacao contexto;
    protected readonly IConectaFormacaoConexao conexao;

    public RepositorioBaseAuditavel(IContextoAplicacao contexto, IConectaFormacaoConexao conexao)
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
        PreencherAuditoriaCriacao(entidade);
        entidade.Id = (long)await conexao.Obter().InsertAsync(entidade);
        return entidade.Id;
    }

    public async Task<TEntidade> Atualizar(TEntidade entidade)
    {
        PreencherAuditoriaAlteracao(entidade);
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
}
