using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ValidarEncontrosProfissionaisAoEnviarDfCommand : IRequest<IEnumerable<string>>
    {
        public ValidarEncontrosProfissionaisAoEnviarDfCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ValidarEncontrosProfissionaisAoEnviarDfCommandValidator : AbstractValidator<ValidarEncontrosProfissionaisAoEnviarDfCommand>
    {
        public ValidarEncontrosProfissionaisAoEnviarDfCommandValidator()
        {
            RuleFor(x => x.Proposta).NotNull().WithMessage("Informe a Proposta para realizar as validações");
        }
    }
}