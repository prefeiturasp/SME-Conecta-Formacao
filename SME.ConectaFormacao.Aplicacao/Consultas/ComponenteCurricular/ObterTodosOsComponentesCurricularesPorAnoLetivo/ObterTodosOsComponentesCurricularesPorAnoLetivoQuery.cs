using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsComponentesCurricularesPorAnoLetivoQuery : IRequest<IEnumerable<ComponenteCurricular>>
    {
        public ObterTodosOsComponentesCurricularesPorAnoLetivoQuery(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }

    public class ObterTodosOsComponentesCurricularesQueryValidator : AbstractValidator<ObterTodosOsComponentesCurricularesPorAnoLetivoQuery>
    {
        public ObterTodosOsComponentesCurricularesQueryValidator()
        {
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares");
        }
    }
}