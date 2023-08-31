using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterGrupoUsuarioLogadoQueryHandlerFaker : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(PropostaSalvarMock.GrupoUsuarioLogadoId);
        }
    }
}
