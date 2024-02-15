using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostasDashboard : CasoDeUsoAbstrato, ICasoDeUsoObterPropostasDashboard
    {
        public CasoDeUsoObterPropostasDashboard(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<PropostaDashboardDTO>> Executar(PropostaFiltrosDashboardDTO filtro)
        {
            var areaPromotoraUsuarioLogado = await mediator.Send(ObterAreaPromotoraUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new ObterDashBoardQuery(filtro, areaPromotoraUsuarioLogado?.Id));
        }
    }
}