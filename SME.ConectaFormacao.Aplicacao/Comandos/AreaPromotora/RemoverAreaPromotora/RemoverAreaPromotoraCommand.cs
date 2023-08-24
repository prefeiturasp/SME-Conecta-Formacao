using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverAreaPromotoraCommand : IRequest<bool>
    {
        public RemoverAreaPromotoraCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class RemoverAreaPromotoraCommandValidator : AbstractValidator<RemoverAreaPromotoraCommand>
    {
        public RemoverAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É nescessário informar o id da área promotora para remover");
        }
    }
}
