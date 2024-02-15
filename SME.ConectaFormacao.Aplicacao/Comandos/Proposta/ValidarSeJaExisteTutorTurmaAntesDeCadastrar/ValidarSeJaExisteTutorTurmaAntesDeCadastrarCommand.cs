using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand(string? registroFuncional, string? nomeTutor, long[] turmaIds)
        {
            RegistroFuncional = registroFuncional;
            NomeTutor = nomeTutor;
            TurmaIds = turmaIds;
        }

        public string? RegistroFuncional { get; }
        public string? NomeTutor { get; }
        public long[] TurmaIds { get; }
    }

    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.TurmaIds).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}