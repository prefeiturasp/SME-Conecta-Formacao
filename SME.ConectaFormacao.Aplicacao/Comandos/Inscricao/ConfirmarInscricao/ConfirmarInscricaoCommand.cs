using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ConfirmarInscricaoCommand : IRequest<bool>
    {
        public ConfirmarInscricaoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ConfirmarInscricaoCommandValidator : AbstractValidator<ConfirmarInscricaoCommand>
    {
        public ConfirmarInscricaoCommandValidator()
        {
            RuleFor(f => f.Id).NotEmpty().WithMessage("Informe o id da inscrição para confirmar");
        }
    }
}
