using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand(string? registroFuncional, string? cpf, string? nomeTutor, long[] turmaIds)
        {
            RegistroFuncional = registroFuncional;
            Cpf = cpf;
            NomeTutor = nomeTutor;
            TurmaIds = turmaIds;
        }

        public string? RegistroFuncional { get; }
        public string? Cpf { get; }
        public string? NomeTutor { get; }
        public long[] TurmaIds { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.TurmaIds).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}