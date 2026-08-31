using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class RemoverPropostaEncontroCommand : IRequest<bool>
    {
        public RemoverPropostaEncontroCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class RemoverPropostaEncontroCommandValidator : AbstractValidator<RemoverPropostaEncontroCommand>
    {
        public RemoverPropostaEncontroCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É nescessário informar o id do encontro para ser removido");
        }
    }
}
