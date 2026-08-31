using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class EnviarParecerAdminDFCommand : IRequest<bool>
    {
        public EnviarParecerAdminDFCommand(long idProposta)
        {
            IdProposta = idProposta;
        }
        public long IdProposta { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EnviarParecerAdminDFCommandValidator : AbstractValidator<EnviarParecerAdminDFCommand>
    {
        public EnviarParecerAdminDFCommandValidator()
        {
            RuleFor(x => x.IdProposta)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para enviar parecer do admin DF");
        }
    }
}
