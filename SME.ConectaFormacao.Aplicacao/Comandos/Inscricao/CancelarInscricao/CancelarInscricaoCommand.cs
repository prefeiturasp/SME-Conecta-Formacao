using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class CancelarInscricaoCommand : IRequest<bool>
    {
        public CancelarInscricaoCommand(long id, string motivo)
        {
            Id = id;
            Motivo = motivo;
        }

        public long Id { get; }
        public string Motivo { get; }
    }

    public class CancelarInscricaoCommandValidator : AbstractValidator<CancelarInscricaoCommand>
    {
        public CancelarInscricaoCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");

            RuleFor(t => t.Motivo)
                .MaximumLength(1000)
                .WithMessage("Motivo do cancelamento não pode conter mais que 1000 caracteres")
                .When(t => t.Motivo.EstaPreenchido());
        }
    }
}
