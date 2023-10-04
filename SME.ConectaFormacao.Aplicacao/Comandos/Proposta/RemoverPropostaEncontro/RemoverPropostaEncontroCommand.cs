using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaEncontroCommand : IRequest<bool>
    {
        public RemoverPropostaEncontroCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }

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
