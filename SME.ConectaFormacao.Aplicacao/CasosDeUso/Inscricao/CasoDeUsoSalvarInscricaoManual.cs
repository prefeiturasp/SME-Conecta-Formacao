using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoSalvarInscricaoManual : CasoDeUsoAbstrato, ICasoDeUsoSalvarInscricaoManual
    {
        public CasoDeUsoSalvarInscricaoManual(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(InscricaoManualDTO inscricaoManualDTO)
        {
            return await mediator.Send(new SalvarInscricaoManualCommand(inscricaoManualDTO, false));
        }
    }
}
