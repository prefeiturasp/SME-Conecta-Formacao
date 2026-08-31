using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class RemoverPropostaRegenteCommand : IRequest<bool>
    {
        public RemoverPropostaRegenteCommand(long regenteId)
        {
            RegenteId = regenteId;
        }

        public long RegenteId { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class RemoverPropostaRegenteCommandValidator : AbstractValidator<RemoverPropostaRegenteCommand>
    {
        public RemoverPropostaRegenteCommandValidator()
        {
            RuleFor(x => x.RegenteId)
                .NotEmpty()
                .WithMessage("É nescessário informar o id do regente para ser removido");
        }
    }
}