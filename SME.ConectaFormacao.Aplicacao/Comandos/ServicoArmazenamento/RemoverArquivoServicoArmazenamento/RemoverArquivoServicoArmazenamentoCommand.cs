using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class RemoverArquivoServicoArmazenamentoCommand : IRequest<bool>
    {
        public RemoverArquivoServicoArmazenamentoCommand(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; }
    }

    [ExcludeFromCodeCoverage]
    public class RemoverArquivoServicoArmazenamentoCommandValidator : AbstractValidator<RemoverArquivoServicoArmazenamentoCommand>
    {
        public RemoverArquivoServicoArmazenamentoCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do arquivo para ser removido");
        }
    }
}
