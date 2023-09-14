using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivosCommand : IRequest<bool>
    {
        public RemoverArquivosCommand(IEnumerable<Arquivo> arquivos)
        {
            Arquivos = arquivos;
        }

        public IEnumerable<Arquivo> Arquivos { get; }
    }

    public class RemoverArquivoCommandValidator : AbstractValidator<RemoverArquivosCommand>
    {
        public RemoverArquivoCommandValidator()
        {
            RuleFor(x => x.Arquivos)
                .NotEmpty()
                .WithMessage("É nescessário informar ao menos um arquivos para ser removido");
        }
    }
}
