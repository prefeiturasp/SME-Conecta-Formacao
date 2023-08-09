using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes;

public class ServicoAcessosFake : IServicoAcessos
{
    public Task<AcessosUsuarioAutenticacaoRetorno> Autenticar(string login, string senha)
    {
        return Task.FromResult(new AcessosUsuarioAutenticacaoRetorno()
        {

        });
    }

    public Task<AcessosPerfisUsuarioRetorno> ObterPerfisUsuario(string login)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UsuarioCadastradoCoreSSO(string login)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CadastrarUsuarioCoreSSO(string login, string nome, string email, string senha)
    {
        return Task.FromResult(true);
    }

    public Task<bool> VincularPerfilExternoCoreSSO(string login, Guid perfilId)
    {
        return Task.FromResult(true);
    }

    public Task<bool> AlterarSenha(string login, string senhaAtual, string senhaNova)
    {
        return Task.FromResult(true);
    }

    public Task<bool> AlterarEmail(string login, string email)
    {
        return Task.FromResult(true);
    }

    public Task<AcessosDadosUsuario> ObterMeusDados(string login)
    {
        return Task.FromResult(new AcessosDadosUsuario());
    }

    public Task<string> SolicitarRecuperacaoSenha(string login)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<bool> TokenRecuperacaoSenhaEstaValido(Guid token)
    {
        return Task.FromResult(true);
    }

    public Task<string> AlterarSenhaComTokenRecuperacao(AcessosRecuperacaoSenha recuperacaoSenhaDto)
    {
        return Task.FromResult(string.Empty);
    }
}

