using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao
{
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
                .When(t => !string.IsNullOrWhiteSpace(t.Motivo));
        }
    }
}
