using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.PropostaGrupoPeriodoPorPropostaId
{
    public class PropostaGrupoPeriodoPorPropostaIdQuery(long propostaId) : IRequest<IEnumerable<PropostaGrupoPeriodoDto>>
    {
        public long PropostaId { get; set; } = propostaId;
    }
}