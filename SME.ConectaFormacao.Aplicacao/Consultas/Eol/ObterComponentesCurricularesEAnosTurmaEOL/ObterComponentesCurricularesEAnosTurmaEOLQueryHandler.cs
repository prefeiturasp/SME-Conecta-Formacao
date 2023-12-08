using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterComponentesCurricularesEAnosTurmaEOLQueryHandler : IRequestHandler<ObterComponentesCurricularesEAnosTurmaEOLQuery, IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>>
{
    private readonly IServicoEol _servicoEol;

    public ObterComponentesCurricularesEAnosTurmaEOLQueryHandler(IServicoEol servicoEol)
    {
        _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
    }

    public async Task<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>> Handle(ObterComponentesCurricularesEAnosTurmaEOLQuery request, CancellationToken cancellationToken)
    {
        return await _servicoEol.ObterComponentesCurricularesEAnoPorAnoLetivo(request.AnoLetivo);
    }
}