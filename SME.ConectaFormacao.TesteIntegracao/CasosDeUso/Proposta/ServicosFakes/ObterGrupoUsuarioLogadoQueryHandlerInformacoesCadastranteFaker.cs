using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterGrupoUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoGrupoId);
        }
    }
}
