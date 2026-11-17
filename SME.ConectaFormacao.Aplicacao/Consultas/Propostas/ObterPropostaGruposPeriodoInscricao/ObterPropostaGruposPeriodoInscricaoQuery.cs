using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGruposPeriodoInscricao
{
    public class ObterPropostaGruposPeriodoInscricaoQuery(long propostaId) : IRequest<IEnumerable<PropostaGrupoPeriodoInscricao>>
    {
        public long PropostaId { get; set; } = propostaId;
    }
}
