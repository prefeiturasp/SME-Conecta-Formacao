using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao;

public class InserirComponenteCurricularCommand : IRequest<long>
{
    public InserirComponenteCurricularCommand(ComponenteCurricular componenteCurricular)
    {
        ComponenteCurricular = componenteCurricular;
    }
    public ComponenteCurricular ComponenteCurricular { get; set; }

    public class InserirComponenteCurricularCommandValidator : AbstractValidator<InserirComponenteCurricularCommand>
    {
        public InserirComponenteCurricularCommandValidator()
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
                .Equal(0)
                .WithMessage("Não pode ser informado o identificador do componente curricular no processo de inclusão de componente curricular");
        }
    }
}