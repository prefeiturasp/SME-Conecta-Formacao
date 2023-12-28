using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CancelarInscricaoCommand : IRequest<bool>
    {
        public CancelarInscricaoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class CancelarInscricaoCommandValidator : AbstractValidator<CancelarInscricaoCommand>
    {
        public CancelarInscricaoCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");
        }
    }
}
