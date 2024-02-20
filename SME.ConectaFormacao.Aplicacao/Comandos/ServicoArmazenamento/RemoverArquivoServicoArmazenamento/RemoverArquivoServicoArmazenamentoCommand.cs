using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivoServicoArmazenamentoCommand : IRequest<bool>
    {
        public RemoverArquivoServicoArmazenamentoCommand(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; }
    }

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
