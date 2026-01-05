using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterGrupoUsuarioLogadoQueryHandlerInformacoesCadastranteFaker : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaInformacoesCadastranteMock.UsuarioLogadoGrupoId);
        }
    }
}
