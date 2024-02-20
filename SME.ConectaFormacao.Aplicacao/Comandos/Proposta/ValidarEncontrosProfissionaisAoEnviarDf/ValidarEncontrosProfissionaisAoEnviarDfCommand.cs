using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEncontrosProfissionaisAoEnviarDfCommand : IRequest<IEnumerable<string>>
    {
        public ValidarEncontrosProfissionaisAoEnviarDfCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; set; }
    }

    public class ValidarEncontrosProfissionaisAoEnviarDfCommandValidator : AbstractValidator<ValidarEncontrosProfissionaisAoEnviarDfCommand>
    {
        public ValidarEncontrosProfissionaisAoEnviarDfCommandValidator()
        {
            RuleFor(x => x.Proposta).NotNull().WithMessage("Informe a Proposta para realizar as validações");
        }
    }
}