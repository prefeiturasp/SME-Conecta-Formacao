using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Modalidade
{
    public class CasoDeUsoObterModalidade : CasoDeUsoAbstrato, ICasoDeUsoObterModalidade
    {
        public CasoDeUsoObterModalidade(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            return await mediator.Send(ObterModalidadesQuery.Instancia);
        }
    }
}
