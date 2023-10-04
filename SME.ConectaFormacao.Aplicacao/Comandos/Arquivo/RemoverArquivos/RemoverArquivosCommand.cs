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
