using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoManual
{
    public class SalvarInscricaoManualCommand(InscricaoManualDTO inscricaoManualDTO, bool ehTransferencia) : IRequest<RetornoDTO>
    {
        public InscricaoManualDTO InscricaoManualDTO { get; } = inscricaoManualDTO;
        public bool EhTransferencia { get; set; } = ehTransferencia;
    }
}