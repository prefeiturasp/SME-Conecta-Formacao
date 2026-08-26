using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao;

[ExcludeFromCodeCoverage]
public class AlterarComponenteCurricularCommand : IRequest<bool>
{
    public AlterarComponenteCurricularCommand(ComponenteCurricular componenteCurricular)
    {
        ComponenteCurricular = componenteCurricular;
    }
    public ComponenteCurricular ComponenteCurricular { get; set; }

    [ExcludeFromCodeCoverage]
    public class AlterarComponenteCurricularCommandValidator : AbstractValidator<AlterarComponenteCurricularCommand>
    {
        public AlterarComponenteCurricularCommandValidator()
        {
            RuleFor(f => f.ComponenteCurricular)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o componente curricular para atualizar o componente curricular");

            RuleFor(f => f.ComponenteCurricular.AnoTurmaId)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o identificador do ano da turma para atualizar o componente curricular");

            RuleFor(f => f.ComponenteCurricular.CodigoEOL)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o cÃ³digo eol para atualizar o componente curricular");

            RuleFor(f => f.ComponenteCurricular.Nome)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o nome para atualizar o componente curricular");

            RuleFor(f => f.ComponenteCurricular.Id)
                .GreaterThan(0)
                .WithMessage("Ã‰ necessÃ¡rio informar o identificador do componente curricular para atualizar o componente curricular");
        }
    }
}
