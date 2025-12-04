using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterFormatos(IMediator mediator) : 
        CasoDeUsoAbstrato(mediator), ICasoDeUsoObterFormatos
    {
        public async Task<IEnumerable<RetornoListagemDTO>> Executar(TipoFormacao tipoFormacao)
        {
            return await mediator.Send(new ObterFormatosQuery(tipoFormacao));
        }
    }
}
