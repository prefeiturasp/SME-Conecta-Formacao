using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class RemoverPropostaTutorCommand : IRequest<bool>
    {
        public RemoverPropostaTutorCommand(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class RemoverPropostaTutorCommandValidator : AbstractValidator<RemoverPropostaTutorCommand>
    {
        public RemoverPropostaTutorCommandValidator()
        {
            RuleFor(x => x.TutorId)
                .NotEmpty()
                .WithMessage("É nescessário informar o id do regente para ser removido");
        }
    }
}