using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterTodosOsAnosTurmaPorAnoLetivoQuery : IRequest<IEnumerable<AnoTurma>>
    {
        public ObterTodosOsAnosTurmaPorAnoLetivoQuery(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterTodosOsAnosTurmaQueryValidator : AbstractValidator<ObterTodosOsAnosTurmaPorAnoLetivoQuery>
    {
        public ObterTodosOsAnosTurmaQueryValidator()
        {
            RuleFor(x => x.AnoLetivo)
                .NotEmpty()
                .WithMessage("É necessário informar o ano letivo para obter os anos da turma");
        }
    }
}