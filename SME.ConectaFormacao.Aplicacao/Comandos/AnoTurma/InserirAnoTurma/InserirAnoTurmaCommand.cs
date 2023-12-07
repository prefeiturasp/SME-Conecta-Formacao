using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao;

public class InserirAnoTurmaCommand : IRequest<long>
{
    public InserirAnoTurmaCommand(AnoTurma anoTurma)
    {
        AnoTurma = anoTurma;
    }
    public AnoTurma AnoTurma { get; set; }

    public class InserirAnoTurmaCommandValidator : AbstractValidator<InserirAnoTurmaCommand>
    {
        public InserirAnoTurmaCommandValidator()
        {
            RuleFor(f => f.AnoTurma)
                .NotNull()
                .WithMessage("É necessário informar o ano para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.AnoLetivo)
                .GreaterThan(0)
                .WithMessage("É necessário informar o ano letivo para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.Modalidade)
                .Must(i => Enum.IsDefined(typeof(Modalidade), i))
                .WithMessage("É necessário informar o ano letivo para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.Descricao)
                .NotNull()
                .WithMessage("É necessário informar a descrição para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.CodigoSerieEnsino)
                .NotNull()
                .WithMessage("É necessário informar o código da série ensino para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.CodigoEOL)
                .NotNull()
                .WithMessage("É necessário informar o código eol para inserir ano da turma");
            
            RuleFor(f => f.AnoTurma.Id)
                .Equal(0)
                .WithMessage("Não é necessário informar id do ano turma para inserir ano da turma");

        }
    }
}