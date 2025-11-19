using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.EmEsperaInscricao
{
    public class EmEsperaInscricaoCommandValidator : AbstractValidator<EmEsperaInscricaoCommand>
    {
        public EmEsperaInscricaoCommandValidator()
        {
            RuleFor(t => t.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da inscrição");
        }
    }
}
