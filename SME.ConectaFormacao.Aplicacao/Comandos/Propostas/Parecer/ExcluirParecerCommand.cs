using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ExcluirParecerCommand : IRequest<bool>
    {
        public ExcluirParecerCommand(long parecerId)
        {
            ParecerId = parecerId;
        }

        public long ParecerId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ExcluirParecerCommandValidator : AbstractValidator<ExcluirParecerCommand>
    {
        public ExcluirParecerCommandValidator()
        {
            RuleFor(x => x.ParecerId).GreaterThan(0).WithMessage("Informe o Id do Parecer");
        }
    }
}