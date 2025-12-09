using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDto>
    {
        public async Task<UsuarioAutenticacaoRetornoDto> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            if (UsuarioAlterarEmailValidacaoMock.Login == request.Login && UsuarioAlterarEmailValidacaoMock.Senha == request.Senha)
            {
                return new UsuarioAutenticacaoRetornoDto { Login = request.Login, Nome = "Teste", Email = "teste@teste.com" };
            }
            return new UsuarioAutenticacaoRetornoDto { Login = string.Empty, Nome = string.Empty, Email = string.Empty };
        }
    }
}