using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarAnoTurmaCommand : IRequest<bool>
{
    public AlterarAnoTurmaCommand(AnoTurma anoTurma)
    {
        AnoTurma = anoTurma;
    }
    public AnoTurma AnoTurma { get; set; }

    public class AlterarAnoTurmaCommandValidator : AbstractValidator<AlterarAnoTurmaCommand>
    {
        public AlterarAnoTurmaCommandValidator()
        {
            RuleFor(f => f.AnoTurma)
                .NotNull()
                .WithMessage("É necessário informar o ano turma para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.AnoLetivo)
                .GreaterThan(0)
                .WithMessage("É necessário informar o ano letivo para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.Modalidade)
                .Must(i => Enum.IsDefined(typeof(Modalidade), i))
                .WithMessage("É necessário informar a modalidade para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.Descricao)
                .NotNull()
                .WithMessage("É necessário informar a descrição para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.CodigoSerieEnsino)
                .NotNull()
                .WithMessage("É necessário informar o código da série ensino para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.CodigoEOL)
                .NotNull()
                .WithMessage("É necessário informar o código eol para atualizar o ano da turma");
            
            RuleFor(f => f.AnoTurma.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o id do ano turma para atualizar o ano da turma");
        }
    }
}