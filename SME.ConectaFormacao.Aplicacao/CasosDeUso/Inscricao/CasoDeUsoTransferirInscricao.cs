using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoTransferirInscricao : CasoDeUsoAbstrato, ICasoDeUsoTransferirInscricao
    {
        public CasoDeUsoTransferirInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoInscricaoDTO> Executar(InscricaoTransferenciaDTO inscricaoTransferenciaDTO)
        {
            return await mediator.Send(new TransferirInscricaoCommand(inscricaoTransferenciaDTO));
        }
    }
}
