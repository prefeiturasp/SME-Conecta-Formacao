using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterComponentesCurricularesEAnoTurmaEOLQuery : IRequest<IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>>
{
    public ObterComponentesCurricularesEAnoTurmaEOLQuery(int anoLetivo)
    {
        AnoLetivo = anoLetivo;
    }
    public int AnoLetivo { get; }
}

public class ObterComponentesCurricularesEAnoTurmaEOLQueryValidator : AbstractValidator<ObterComponentesCurricularesEAnoTurmaEOLQuery>
{
    public ObterComponentesCurricularesEAnoTurmaEOLQueryValidator()
    {
        RuleFor(x => x.AnoLetivo)
            .NotEmpty()
            .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares do EOL");
    }
}
