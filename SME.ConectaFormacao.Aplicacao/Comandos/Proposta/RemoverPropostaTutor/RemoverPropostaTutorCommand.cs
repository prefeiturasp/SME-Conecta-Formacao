using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Proposta.RemoverPropostaTutor
{
    public class RemoverPropostaTutorCommand : IRequest<bool>
    {
        public RemoverPropostaTutorCommand(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
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