using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverUsuarioCommand : IRequest<bool>
    {
        public RemoverUsuarioCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class RemoverUsuarioCommandValidator : AbstractValidator<RemoverUsuarioCommand>
    {
        public RemoverUsuarioCommandValidator()
        {
            RuleFor(f => f.Id)
                .NotEmpty()
                .WithMessage("Informe o id do usuário para remover");
        }
    }
}
