using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverArquivoPorIdCommand : IRequest<bool>
    {
        public RemoverArquivoPorIdCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class RemoverArquivoPorIdCommandValidator : AbstractValidator<RemoverArquivoPorIdCommand>
    {
        public RemoverArquivoPorIdCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id do arquivo para remover");
        }
    }
}
