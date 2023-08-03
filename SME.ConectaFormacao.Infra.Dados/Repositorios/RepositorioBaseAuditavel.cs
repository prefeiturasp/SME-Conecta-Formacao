using Dommel;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;

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
        entidade.CriadoEm = DateTimeExtension.HorarioBrasilia();
        entidade.CriadoPor = contexto.NomeUsuario;
        entidade.CriadoLogin = contexto.UsuarioLogado;
        entidade.Id = (long)await conexao.Obter().InsertAsync(entidade);
        return entidade.Id;
    }

    public async Task<TEntidade> Atualizar(TEntidade entidade)
    {
        entidade.AlteradoEm = DateTimeExtension.HorarioBrasilia();
        entidade.AlteradoPor = contexto.NomeUsuario;
        entidade.AlteradoLogin = contexto.UsuarioLogado;
        await conexao.Obter().UpdateAsync(entidade);
        return entidade;
    }

    public async Task Remover(TEntidade entidade)
    {
        throw new NotImplementedException();
    }

    public async Task Remover(long id)
    {
        throw new NotImplementedException();
    }
}
