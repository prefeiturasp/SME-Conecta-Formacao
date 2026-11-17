using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGrupoPeriodoInscricaoPorPropostaId
{
    public class ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQueryHandler(IRepositorioPropostaGrupoPeriodoInscricao repositorioPropostaGrupoPeriodoInscricao)
        : IRequestHandler<ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQuery, IEnumerable<PropostaGrupoPeriodoInscricaoDto>>
    {
        public async Task<IEnumerable<PropostaGrupoPeriodoInscricaoDto>> Handle(ObterPropostaGrupoPeriodoInscricaoPorPropostaIdQuery request, CancellationToken cancellationToken)
        {
            var grupos = await repositorioPropostaGrupoPeriodoInscricao.ObterPorPropostaIdAsync(request.PropostaId);
            return grupos.Select(g => new PropostaGrupoPeriodoInscricaoDto
            {
                Id = g.Id,
                DataInicio = g.DataInicio,
                DataFim = g.DataFim,
                PropostaTurmasIds = [.. g.TurmasVinculadas.Where(t => !t.Excluido).Select(t => t.PropostaTurmaId)]
            });
        }
    }
}
