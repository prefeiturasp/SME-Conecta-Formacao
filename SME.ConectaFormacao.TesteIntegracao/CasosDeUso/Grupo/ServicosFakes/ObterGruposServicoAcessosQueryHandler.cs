using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.ServicosFakes
{
    internal class ObterGruposServicoAcessosQueryHandlerFake : IRequestHandler<ObterGruposServicoAcessosQuery, IEnumerable<GrupoDTO>>
    {
        public Task<IEnumerable<GrupoDTO>> Handle(ObterGruposServicoAcessosQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(AoObterGrupoMock.Grupos);
        }
    }
}
