using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SortearInscricaoCommand : IRequest<bool>
    {
        public SortearInscricaoCommand(long propostaTurmaId)
        {
            PropostaTurmaId = propostaTurmaId;
        }

        public long PropostaTurmaId { get; }
    }

    public class SortearInscricaoCommandValidator : AbstractValidator<SortearInscricaoCommand>
    {
        public SortearInscricaoCommandValidator()
        {
            RuleFor(f => f.PropostaTurmaId)
                .NotEmpty()
                .WithMessage("Informe o id da proposta turma para realizar o sorteio das inscrições");
        }
    }
}
