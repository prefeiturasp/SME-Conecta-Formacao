using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ArmazenarArquivoFisicoServicoArmazenamentoCommand : IRequest<string>
    {
        public ArmazenarArquivoFisicoServicoArmazenamentoCommand(ArquivoDTO arquivo)
        {
            Arquivo = arquivo;
        }

        public ArquivoDTO Arquivo { get; }
    }

    public class ArmazenarArquivoFisicoServicoArmazenamentoCommandValidator : AbstractValidator<ArmazenarArquivoFisicoServicoArmazenamentoCommand>
    {
        public ArmazenarArquivoFisicoServicoArmazenamentoCommandValidator()
        {
            RuleFor(x => x.Arquivo.FormFile)
                .NotEmpty()
                .WithMessage("É nescessário informar o arquivo para o armazenamento físico");
        }
    }
}
