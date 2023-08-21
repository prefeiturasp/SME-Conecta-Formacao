using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Grupo;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo
{
    public class CasoDeUsoObterGrupos : CasoDeUsoAbstrato, ICasoDeUsoObterGrupos
    {
        public CasoDeUsoObterGrupos(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<GrupoDTO>> Executar()
        {
            return await mediator.Send(new ObterGruposServicoAcessosQuery());
        }
    }
}
