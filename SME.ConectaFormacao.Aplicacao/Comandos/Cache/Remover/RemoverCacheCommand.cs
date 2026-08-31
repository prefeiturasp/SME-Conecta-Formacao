using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao;


[ExcludeFromCodeCoverage]
public class RemoverCacheCommand : IRequest
{
    public RemoverCacheCommand(string chave)
    {
        Chave = chave;
    }
    public string Chave { get; set; }


    [ExcludeFromCodeCoverage]
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