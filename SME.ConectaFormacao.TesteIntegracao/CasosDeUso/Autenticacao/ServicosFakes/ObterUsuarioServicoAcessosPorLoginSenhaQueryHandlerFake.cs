using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    internal class ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake : IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>
    {
        public Task<UsuarioAutenticacaoRetornoDTO> Handle(ObterUsuarioServicoAcessosPorLoginSenhaQuery request, CancellationToken cancellationToken)
        {
            if (request.Login == AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login && AutenticacaoMock.AutenticacaoUsuarioDTOValido.Senha == request.Senha)
            {
                return Task.FromResult(AutenticacaoMock.UsuarioAutenticacaoRetornoDTOValido);
            }

            return Task.FromResult(new UsuarioAutenticacaoRetornoDTO());
        }
    }
}
