using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.PropostaGrupoPeriodoPorPropostaId
{
    public class PropostaGrupoPeriodoPorPropostaIdQueryHandler(IRepositorioPropostaGrupoPeriodo repositorioPropostaGrupoPeriodo)
        : IRequestHandler<PropostaGrupoPeriodoPorPropostaIdQuery, IEnumerable<PropostaGrupoPeriodoDto>>
    {
        public async Task<IEnumerable<PropostaGrupoPeriodoDto>> Handle(PropostaGrupoPeriodoPorPropostaIdQuery request, CancellationToken cancellationToken)
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