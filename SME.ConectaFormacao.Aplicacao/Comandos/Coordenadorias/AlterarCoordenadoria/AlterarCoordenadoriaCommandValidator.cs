using FluentValidation;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria
{
    public class AlterarCoordenadoriaCommandValidator : AbstractValidator<AlterarCoordenadoriaCommand>
    {
        public AlterarCoordenadoriaCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id da coordenadoria deve ser maior que zero.");
            RuleFor(c => c.Nome).NotEmpty().WithMessage("Nome da coordenadoria é obrigatório.");
            RuleFor(c => c.Sigla).NotEmpty().WithMessage("Sigla da coordenadoria é obrigatória.")
                .MaximumLength(10).WithMessage("Sigla da coordenadoria deve ter no máximo 10 caracteres.");
        }
    }
}
