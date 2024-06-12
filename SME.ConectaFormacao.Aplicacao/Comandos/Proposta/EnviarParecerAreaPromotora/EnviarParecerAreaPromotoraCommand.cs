using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarParecerAreaPromotoraCommand : IRequest<bool>
    {
        public EnviarParecerAreaPromotoraCommand(long idProposta)
        {
            IdProposta = idProposta;
        }
        public long IdProposta { get; set; }
    }

    public class EnviarParecerAreaPromotoraCommandValidator : AbstractValidator<EnviarParecerAreaPromotoraCommand>
    {
        public EnviarParecerAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.IdProposta)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para enviar parecer pela Área Promotora");
        }
    }
}
