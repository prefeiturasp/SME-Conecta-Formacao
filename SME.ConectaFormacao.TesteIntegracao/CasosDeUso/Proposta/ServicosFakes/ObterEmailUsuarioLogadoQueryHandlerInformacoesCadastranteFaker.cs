using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterEmailUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterEmailUsuarioLogadoQuery, string>
    {
        public Task<string> Handle(ObterEmailUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoEmail);
        }
    }
}
