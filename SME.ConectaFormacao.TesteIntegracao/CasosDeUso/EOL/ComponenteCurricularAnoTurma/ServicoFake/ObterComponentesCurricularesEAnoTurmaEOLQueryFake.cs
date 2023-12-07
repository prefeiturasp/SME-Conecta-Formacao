using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma.Mock;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma
{
    public class ObterComponentesCurricularesEAnoTurmaEOLQueryFake : IRequestHandler<ObterComponentesCurricularesEAnoTurmaEOLQuery, IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>>
    {
        public Task<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>> Handle(ObterComponentesCurricularesEAnoTurmaEOLQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ComponenteCurricularAnoTurmaMock.ComponentesCurricularesAnosTurmas);
        }
    }
}
