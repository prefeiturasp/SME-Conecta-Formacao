using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.ServicosFakes
{
    public class ObterGrupoUsuarioLogadoQueryHandlerFaker : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Perfis.ADMIN_DF);
        }
    }
}
