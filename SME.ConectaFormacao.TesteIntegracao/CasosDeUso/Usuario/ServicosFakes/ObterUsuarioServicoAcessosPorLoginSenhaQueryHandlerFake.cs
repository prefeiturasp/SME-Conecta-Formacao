using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes
{
    public class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery,UsuarioAutenticacaoRetornoDTO>
    {
        public async Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            if (UsuarioAlterarEmailValidacaoMock.Login == request.Login && UsuarioAlterarEmailValidacaoMock.Senha == request.Senha)
            {
                return new UsuarioAutenticacaoRetornoDTO {Login = request.Login, Nome = "Teste", Email = "teste@teste.com"};
            }
            return new UsuarioAutenticacaoRetornoDTO {Login = string.Empty, Nome = string.Empty, Email = string.Empty};
        }
    }
}