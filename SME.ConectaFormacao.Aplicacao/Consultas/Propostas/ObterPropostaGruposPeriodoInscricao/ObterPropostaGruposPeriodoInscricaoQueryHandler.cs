using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterPropostaGruposPeriodoInscricao
{
    public class ObterPropostaGruposPeriodoInscricaoQueryHandler(IRepositorioPropostaGrupoPeriodoInscricao repositorioPropostaGrupoPeriodoInscricao)
        : IRequestHandler<ObterPropostaGruposPeriodoInscricaoQuery, IEnumerable<PropostaGrupoPeriodoInscricao>>
    {
        public async Task<IEnumerable<PropostaGrupoPeriodoInscricao>> Handle(ObterPropostaGruposPeriodoInscricaoQuery request, CancellationToken cancellationToken)
        {
            return await repositorioPropostaGrupoPeriodoInscricao.ObterPorPropostaIdAsync(request.PropostaId);
        }
    }
}
