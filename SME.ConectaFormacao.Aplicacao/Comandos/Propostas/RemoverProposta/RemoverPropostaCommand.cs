using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
    public class RemoverPropostaCommandValidator : AbstractValidator<RemoverPropostaCommand>
    {
        public RemoverPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id para remover a proposta");
        }
    }
}
