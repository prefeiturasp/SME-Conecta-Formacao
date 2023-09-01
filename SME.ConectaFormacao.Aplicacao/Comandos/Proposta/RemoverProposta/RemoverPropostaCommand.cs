using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaCommand : IRequest<bool>
    {
        public RemoverPropostaCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class RemoverPropostaCommandValidator : AbstractValidator<RemoverPropostaCommand>
    {
        public RemoverPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É nescessário informar o Id para remover a proposta");
        }
    }
}
