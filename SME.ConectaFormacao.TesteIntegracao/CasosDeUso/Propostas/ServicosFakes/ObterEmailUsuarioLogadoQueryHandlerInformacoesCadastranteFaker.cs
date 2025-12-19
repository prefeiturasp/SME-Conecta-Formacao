using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterEmailUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterEmailUsuarioLogadoQuery, string>
    {
        public Task<string> Handle(ObterEmailUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoEmail);
        }
    }
}
