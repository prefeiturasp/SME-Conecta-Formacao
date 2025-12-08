using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes
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
