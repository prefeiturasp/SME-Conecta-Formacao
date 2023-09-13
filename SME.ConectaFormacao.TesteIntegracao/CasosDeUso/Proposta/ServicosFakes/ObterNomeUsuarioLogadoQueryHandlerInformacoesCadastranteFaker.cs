using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterNomeUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterNomeUsuarioLogadoQuery, string>
    {
        public Task<string> Handle(ObterNomeUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoNome);
        }
    }
}
