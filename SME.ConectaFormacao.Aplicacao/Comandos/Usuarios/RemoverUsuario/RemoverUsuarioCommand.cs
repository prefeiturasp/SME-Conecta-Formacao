using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class RemoverUsuarioCommand : IRequest<bool>
    {
        public RemoverUsuarioCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    [ExcludeFromCodeCoverage]
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
