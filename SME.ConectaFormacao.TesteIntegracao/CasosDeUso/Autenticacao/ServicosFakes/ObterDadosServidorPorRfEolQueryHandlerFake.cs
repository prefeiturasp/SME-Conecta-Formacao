using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Autenticacao.ServicosFakes
{
    internal class ObterDadosServidorPorRfEolQueryHandlerFake : IRequestHandler<ObterDadosServidorPorRfEolQuery, UsuarioEolDto>
    {
        public Task<UsuarioEolDto> Handle(ObterDadosServidorPorRfEolQuery request, CancellationToken cancellationToken)
        {
            if (request.RfServidor.Equals(AutenticacaoMock.AutenticacaoUsuarioDTOValido.Login))
                return Task.FromResult(new UsuarioEolDto { Nome = AutenticacaoMock.UsuarioPerfisRetornoDTOValido.UsuarioNome });

            return Task.FromResult(request.RfServidor.Equals(AutenticacaoMock.UsuarioLogado.Login)
                ? new UsuarioEolDto { Nome = AutenticacaoMock.UsuarioLogado.Nome }
                : new UsuarioEolDto { Nome = string.Empty });
        }
    }
}
