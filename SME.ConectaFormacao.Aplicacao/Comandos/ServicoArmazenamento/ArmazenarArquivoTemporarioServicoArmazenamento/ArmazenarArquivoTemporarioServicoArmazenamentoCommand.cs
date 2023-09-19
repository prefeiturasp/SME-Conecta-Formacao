using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommand : IRequest<string>
    {
        public ArmazenarArquivoTemporarioServicoArmazenamentoCommand(ArquivoDTO arquivo)
        {
            Arquivo = arquivo;
        }

        public ArquivoDTO Arquivo { get; }
    }

    public class ArmazenarArquivoTemporarioServicoArmazenamentoCommandValidator : AbstractValidator<ArmazenarArquivoTemporarioServicoArmazenamentoCommand>
    {
        public ArmazenarArquivoTemporarioServicoArmazenamentoCommandValidator()
        {
            RuleFor(x => x.Arquivo.FormFile)
                .NotEmpty()
                .WithMessage("É nescessário informar o arquivo para o armazenamento físico");
        }
    }
}
