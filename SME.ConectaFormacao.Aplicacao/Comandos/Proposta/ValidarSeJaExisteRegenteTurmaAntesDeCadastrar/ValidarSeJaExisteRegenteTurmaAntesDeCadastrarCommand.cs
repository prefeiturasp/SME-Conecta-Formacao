using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(string? registroFuncional, string? nomeRegente, long[] turmaIds)
        {
            RegistroFuncional = registroFuncional;
            NomeRegente = nomeRegente;
            TurmaIds = turmaIds;
        }

        public string? RegistroFuncional { get; }
        public string? NomeRegente { get; }
        public long[] TurmaIds { get; }
    }

    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator : AbstractValidator<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandValidator()
        {
            RuleFor(x => x.TurmaIds).NotEmpty().WithMessage("Informe as Turmas");
        }
    }
}