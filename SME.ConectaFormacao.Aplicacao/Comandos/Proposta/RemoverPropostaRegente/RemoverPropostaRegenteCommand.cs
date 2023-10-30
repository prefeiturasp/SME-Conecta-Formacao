using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Proposta.RemoverPropostaRegente
{
    public class RemoverPropostaRegenteCommand : IRequest<bool>
    {
        public RemoverPropostaRegenteCommand(long regenteId)
        {
            RegenteId = regenteId;
        }

        public long RegenteId { get; set; }
    }
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