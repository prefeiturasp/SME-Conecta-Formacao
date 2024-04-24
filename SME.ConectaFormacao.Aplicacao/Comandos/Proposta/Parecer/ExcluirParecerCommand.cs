using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExcluirParecerCommand : IRequest<bool>
    {
        public ExcluirParecerCommand(long parecerId)
        {
            ParecerId = parecerId;
        }

        public long ParecerId { get; set; }
    }

    public class ExcluirParecerCommandValidator : AbstractValidator<ExcluirParecerCommand>
    {
        public ExcluirParecerCommandValidator()
        {
            RuleFor(x => x.ParecerId).GreaterThan(0).WithMessage("Informe o Id do Parecer");
        }
    }
}