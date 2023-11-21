using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo
{
    public class CasoDeUsoObterGrupoGestao : CasoDeUsoAbstrato, ICasoDeUsoObterGrupoGestao
    {
        public CasoDeUsoObterGrupoGestao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<GrupoDTO>> Executar()
        {
            var gruposCoreSSO = await mediator.Send(new ObterGruposServicoAcessosQuery());

            var grupoGestao = await mediator.Send(new ObterGruposGestaoAcessosQuery());
            
            return grupoGestao.Intersect(gruposCoreSSO);
        }
    }
}
