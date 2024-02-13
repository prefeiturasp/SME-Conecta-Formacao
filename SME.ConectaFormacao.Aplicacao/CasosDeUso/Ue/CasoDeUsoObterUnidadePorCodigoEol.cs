using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Ue
{
    public class CasoDeUsoObterUnidadePorCodigoEol : CasoDeUsoAbstrato, ICasoDeUsoObterUnidadePorCodigoEol
    {

        public CasoDeUsoObterUnidadePorCodigoEol(IMediator mediator) : base(mediator) { }

        public async Task<UnidadeEol> Executar(string codigoEolUnidade)
        {
            return await mediator.Send(new ObterUnidadePorCodigoEOLQuery(codigoEolUnidade));
        }
    }
}