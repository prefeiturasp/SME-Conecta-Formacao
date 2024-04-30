using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerPareceristaCommand : IRequest<bool>
    {
        public EnviarParecerPareceristaCommand(long idProposta)
        {
            IdProposta = idProposta;
        }
        public long IdProposta { get; set; }
    }

    public class EnviarParecerPareceristasCommandValidator : AbstractValidator<EnviarParecerPareceristaCommand>
    {
        public EnviarParecerPareceristasCommandValidator()
        {
            RuleFor(x => x.IdProposta)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para enviar parecer do parecerista");
        }
    }
}
