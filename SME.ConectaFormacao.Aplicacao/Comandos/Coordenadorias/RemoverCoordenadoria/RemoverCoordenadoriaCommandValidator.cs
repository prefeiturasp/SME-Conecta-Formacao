using FluentValidation;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria
{
    public class RemoverCoordenadoriaCommandValidator : AbstractValidator<RemoverCoordenadoriaCommand>
    {
        public RemoverCoordenadoriaCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id da coordenadoria deve ser maior que zero.");
        }
    }
}