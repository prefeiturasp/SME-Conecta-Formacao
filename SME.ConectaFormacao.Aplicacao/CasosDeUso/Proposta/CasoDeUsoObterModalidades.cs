using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterModalidades : CasoDeUsoAbstrato, ICasoDeUsoObterModalidades
    {
        public CasoDeUsoObterModalidades(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar(TipoFormacao tipoFormacaoId)
        {
            return await mediator.Send(new ObterModalidadesQuery(tipoFormacaoId));
        }
    }
}
