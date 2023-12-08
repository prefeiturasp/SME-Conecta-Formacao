using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma.Mock;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma
{
    public class ObterComponentesCurricularesEAnoTurmaEOLQueryFake : IRequestHandler<ObterComponentesCurricularesEAnosTurmaEOLQuery, IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>>
    {
        public Task<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>> Handle(ObterComponentesCurricularesEAnosTurmaEOLQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ComponenteCurricularAnoTurmaMock.ComponentesCurricularesAnosTurmas);
        }
    }
}
