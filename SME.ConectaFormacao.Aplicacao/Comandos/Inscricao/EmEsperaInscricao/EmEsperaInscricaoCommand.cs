using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EmEsperaInscricaoCommand : IRequest<bool>
    {
        public EmEsperaInscricaoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class EmEsperaInscricaoCommandValidator : AbstractValidator<EmEsperaInscricaoCommand>
    {
        public EmEsperaInscricaoCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");
        }
    }
}
