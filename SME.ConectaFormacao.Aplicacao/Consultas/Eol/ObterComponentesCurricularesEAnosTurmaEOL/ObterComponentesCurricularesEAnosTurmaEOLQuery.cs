using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao;

public class ObterComponentesCurricularesEAnosTurmaEOLQuery : IRequest<IEnumerable<ComponenteCurricularAnoTurmaServicoEol>>
{
    public ObterComponentesCurricularesEAnosTurmaEOLQuery(int anoLetivo)
    {
        AnoLetivo = anoLetivo;
    }
    public int AnoLetivo { get; }
}

public class ObterComponentesCurricularesEAnoTurmaEOLQueryValidator : AbstractValidator<ObterComponentesCurricularesEAnosTurmaEOLQuery>
{
    public ObterComponentesCurricularesEAnoTurmaEOLQueryValidator()
    {
        RuleFor(x => x.AnoLetivo)
            .NotEmpty()
            .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares dos anos das turmas do EOL");
    }
}
