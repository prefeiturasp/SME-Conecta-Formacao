using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand : IRequest<string>
    {
        public MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; }
    }

    public class MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommandValidator : AbstractValidator<MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommand>
    {
        public MoverArquivoTemporarioParaFisicoServicoArmazenamentoCommandValidator()
        {
            RuleFor(x => x.Nome)
               .NotEmpty()
               .WithMessage("É nescessário informar o nome do arquivo para ser mover para o repositório fisico");
        }
    }
}
