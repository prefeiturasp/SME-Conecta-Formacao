using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao;

public class RemoverCacheCommand : IRequest
{
    public RemoverCacheCommand(string chave)
    {
        Chave = chave;
    }
    public string Chave { get; set; }

    public class RemoverCacheCommandValidator : AbstractValidator<RemoverCacheCommand>
    {
        public RemoverCacheCommandValidator()
        {
            RuleFor(f => f.Chave)
                .NotNull()
                .WithMessage("É necessário informar a chave para remover o cache");
        }
    }
}