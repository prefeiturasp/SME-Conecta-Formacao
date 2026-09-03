using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{

    [ExcludeFromCodeCoverage]
    public class RemoverArquivosCommand : IRequest<bool>
    {
        public RemoverArquivosCommand(IEnumerable<Arquivo> arquivos)
        {
            Arquivos = arquivos;
        }

        public IEnumerable<Arquivo> Arquivos { get; }
    }


    [ExcludeFromCodeCoverage]
    public class RemoverArquivosCommandValidator : AbstractValidator<RemoverArquivosCommand>
    {
        public RemoverArquivosCommandValidator()
        {
            RuleFor(x => x.Arquivos)
                .NotEmpty()
                .WithMessage("É necessário informar ao menos um arquivos para ser removido");
        }
    }
}
