using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo
{
    public class CasoDeUsoObterGrupoSistema : CasoDeUsoAbstrato, ICasoDeUsoObterGrupoSistema
    {
        public CasoDeUsoObterGrupoSistema(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<GrupoDTO>> Executar()
        {
            return await mediator.Send(new ObterGruposServicoAcessosQuery());
        }
    }
}
