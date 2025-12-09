using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoManual;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
{
    public class CasoDeUsoSalvarInscricaoManual(IMediator mediator) : CasoDeUsoAbstrato(mediator), ICasoDeUsoSalvarInscricaoManual
    {
        public async Task<RetornoDTO> Executar(InscricaoManualDTO inscricaoManualDTO)
        {
            return await mediator.Send(new SalvarInscricaoManualCommand(inscricaoManualDTO, false));
        }
    }
}
