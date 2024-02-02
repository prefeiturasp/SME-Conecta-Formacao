using MediatR;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ue;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Ue
{
    public class CasoDeUsoObterUePorCodigo : CasoDeUsoAbstrato, ICasoDeUsoObterUePorCodigo
    {

        public CasoDeUsoObterUePorCodigo(IMediator mediator) : base(mediator){}

        public async Task<UeServicoEol> Executar(string ueCodigo)
        {
            return await mediator.Send(new ObterUePorCodigoEOLQuery(ueCodigo));
        }
    }
}