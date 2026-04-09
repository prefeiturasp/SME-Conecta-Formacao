using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoPorPropostaId
{
    public class ObterPropostaGrupoPeriodoPorPropostaIdQueryHandler(IRepositorioPropostaGrupoPeriodo repositorioPropostaGrupoPeriodo)
        : IRequestHandler<ObterPropostaGrupoPeriodoPorPropostaIdQuery, IEnumerable<PropostaGrupoPeriodoDto>>
    {
        public async Task<IEnumerable<PropostaGrupoPeriodoDto>> Handle(ObterPropostaGrupoPeriodoPorPropostaIdQuery request, CancellationToken cancellationToken)
        {
            var grupos = await repositorioPropostaGrupoPeriodo.ObterPorPropostaIdAsync(request.PropostaId);
            return grupos.Select(g => new PropostaGrupoPeriodoDto
            {
                Id = g.Id,
                DataInicio = g.DataInicio,
                DataFim = g.DataFim,
                PropostaTurmasIds = [.. g.TurmasVinculadas.Select(t => t.PropostaTurmaId)]
            });
        }
    }
}