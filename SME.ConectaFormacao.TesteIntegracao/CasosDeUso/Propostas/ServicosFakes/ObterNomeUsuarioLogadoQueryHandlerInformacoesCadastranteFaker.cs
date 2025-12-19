using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterNomeUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterNomeUsuarioLogadoQuery, string>
    {
        public Task<string> Handle(ObterNomeUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoNome);
        }
    }
}
