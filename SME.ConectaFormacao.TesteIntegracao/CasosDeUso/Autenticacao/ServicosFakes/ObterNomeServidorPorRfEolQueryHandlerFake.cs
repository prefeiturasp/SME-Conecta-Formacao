using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    internal class ObterNomeServidorPorRfEolQueryHandlerFake : IRequestHandler<ObterNomeServidorPorRfEolQuery, string>
    {
        public Task<string> Handle(ObterNomeServidorPorRfEolQuery request, CancellationToken cancellationToken)
        {
            if (request.RfServidor.Equals(AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login))
                return Task.FromResult(AutenticacaoMock.UsuarioPerfisRetornoDTOValido.UsuarioNome);

            return Task.FromResult(request.RfServidor.Equals(AutenticacaoMock.UsuarioLogado.Login) 
                ? AutenticacaoMock.UsuarioLogado.Nome  
                : string.Empty);
        }
    }
}
