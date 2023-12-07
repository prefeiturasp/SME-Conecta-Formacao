using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarComponenteCurricularCommand : IRequest<bool>
{
    public AlterarComponenteCurricularCommand(ComponenteCurricular componenteCurricular)
    {
        ComponenteCurricular = componenteCurricular;
    }
    public ComponenteCurricular ComponenteCurricular { get; set; }

    public class AlterarComponenteCurricularCommandValidator : AbstractValidator<AlterarComponenteCurricularCommand>
    {
            public AlterarComponenteCurricularCommandValidator()
            {
                RuleFor(f => f.ComponenteCurricular)
                    .NotNull()
                    .WithMessage("É necessário informar o componente curricular para a inserir o componente curricular");
                    
                RuleFor(f => f.ComponenteCurricular.AnoTurmaId)
                    .NotNull()
                    .WithMessage("É necessário informar o identificador do ano da turma para a inserir o componente curricular");
                
                RuleFor(f => f.ComponenteCurricular.CodigoEOL)
                    .NotNull()
                    .WithMessage("É necessário informar o código eol para a inserir o componente curricular");
                
                RuleFor(f => f.ComponenteCurricular.Nome)
                    .NotNull()
                    .WithMessage("É necessário informar o nome para a inserir o componente curricular");
                
                RuleFor(f => f.ComponenteCurricular.Id)
                    .GreaterThan(0)
                    .WithMessage("É necessário informar o identificador do componente curricular no processo de atualização de componente curricular");
            }
    }
}