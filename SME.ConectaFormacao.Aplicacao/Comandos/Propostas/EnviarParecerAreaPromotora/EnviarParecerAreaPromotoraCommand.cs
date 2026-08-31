using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class EnviarParecerAreaPromotoraCommand : IRequest<bool>
    {
        public EnviarParecerAreaPromotoraCommand(long idProposta)
        {
            IdProposta = idProposta;
        }
        public long IdProposta { get; set; }
    }

    [ExcludeFromCodeCoverage]
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
