using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterFormatos : CasoDeUsoAbstrato, ICasoDeUsoObterFormatos
    {
        public CasoDeUsoObterFormatos(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar(TipoFormacao tipoFormacaoId)
        {
            return await mediator.Send(new ObterFormatosQuery(tipoFormacaoId));
        }
    }
}
