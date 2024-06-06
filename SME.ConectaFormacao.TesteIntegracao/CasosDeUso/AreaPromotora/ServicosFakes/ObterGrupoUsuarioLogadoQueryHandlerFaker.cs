using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.ServicosFakes
{
    public class ObterGrupoUsuarioLogadoQueryHandlerFaker : IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>
    {
        public Task<Guid> Handle(ObterGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Perfis.ADMIN_DF);
        }
    }
}
