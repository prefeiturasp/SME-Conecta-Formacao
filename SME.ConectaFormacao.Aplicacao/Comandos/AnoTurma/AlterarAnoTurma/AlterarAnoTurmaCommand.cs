using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao;

[ExcludeFromCodeCoverage]
    public class AlterarAnoTurmaCommand : IRequest<bool>
{
    public AlterarAnoTurmaCommand(AnoTurma anoTurma)
    {
        AnoTurma = anoTurma;
    }
    public AnoTurma AnoTurma { get; set; }

    [ExcludeFromCodeCoverage]
    public class AlterarAnoTurmaCommandValidator : AbstractValidator<AlterarAnoTurmaCommand>
    {
        public AlterarAnoTurmaCommandValidator()
        {
            RuleFor(f => f.AnoTurma)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o ano turma para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.AnoLetivo)
                .GreaterThan(0)
                .WithMessage("Ã‰ necessÃ¡rio informar o ano letivo para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.Modalidade)
                .Must(i => Enum.IsDefined(typeof(Modalidade), i))
                .WithMessage("Ã‰ necessÃ¡rio informar a modalidade para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.Descricao)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar a descriÃ§Ã£o para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.CodigoSerieEnsino)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o cÃ³digo da sÃ©rie ensino para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.CodigoEOL)
                .NotNull()
                .WithMessage("Ã‰ necessÃ¡rio informar o cÃ³digo eol para atualizar o ano da turma");

            RuleFor(f => f.AnoTurma.Id)
                .GreaterThan(0)
                .WithMessage("Ã‰ necessÃ¡rio informar o id do ano turma para atualizar o ano da turma");
        }
    }
}
