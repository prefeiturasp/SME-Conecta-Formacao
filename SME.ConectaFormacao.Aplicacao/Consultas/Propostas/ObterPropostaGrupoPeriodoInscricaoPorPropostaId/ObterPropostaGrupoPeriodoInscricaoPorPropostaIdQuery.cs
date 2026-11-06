using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoInscricaoPorPropostaId
{
    public class ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQuery(long propostaId) : IRequest<IEnumerable<PropostaGrupoPeriodoInscricaoDto>>
    {
        public long PropostaId { get; set; } = propostaId;
    }
}
