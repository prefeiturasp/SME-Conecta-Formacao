using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand : IRequest
    {
        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(string? registroFuncional, string? cpf, string? nomeRegente, long[] turmaIds)
        {
            RegistroFuncional = registroFuncional;
            Cpf = cpf;
            NomeRegente = nomeRegente;
            TurmaIds = turmaIds;
        }

        public string? RegistroFuncional { get; }
        public string? NomeRegente { get; }
        public string? Cpf { get; }
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