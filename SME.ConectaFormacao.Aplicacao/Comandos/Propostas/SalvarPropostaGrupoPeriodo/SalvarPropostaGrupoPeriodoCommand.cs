using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo
{
    public class SalvarPropostaGrupoPeriodoCommand(long propostaId, PropostaDTO propostaDto) : IRequest<Resultado>
    {
        public long PropostaId { get; set; } = propostaId;
        public PropostaDTO PropostaDto { get; set; } = propostaDto;
    }
}