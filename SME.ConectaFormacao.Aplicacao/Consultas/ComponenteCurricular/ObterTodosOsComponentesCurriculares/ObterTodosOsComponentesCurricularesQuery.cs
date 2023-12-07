using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsComponentesCurricularesQuery : IRequest<IEnumerable<ComponenteCurricular>>
    {
        public ObterTodosOsComponentesCurricularesQuery(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }

    public class ObterTodosOsComponentesCurricularesQueryValidator : AbstractValidator<ObterTodosOsComponentesCurricularesQuery>
    {
        public ObterTodosOsComponentesCurricularesQueryValidator()
        {
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os componentes curriculares");
        }
    }
}