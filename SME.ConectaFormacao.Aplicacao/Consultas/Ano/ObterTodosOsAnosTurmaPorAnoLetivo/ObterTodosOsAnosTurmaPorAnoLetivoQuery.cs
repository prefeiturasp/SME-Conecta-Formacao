using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTodosOsAnosTurmaPorAnoLetivoQuery : IRequest<IEnumerable<AnoTurma>>
    {
        public ObterTodosOsAnosTurmaPorAnoLetivoQuery(int anoLetivo)
        {
            AnoLetivo = anoLetivo;
        }

        public int AnoLetivo { get; set; }
    }

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