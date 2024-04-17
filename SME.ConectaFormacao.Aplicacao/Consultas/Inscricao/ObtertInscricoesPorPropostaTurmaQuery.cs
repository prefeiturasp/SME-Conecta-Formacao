using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObtertInscricoesPorPropostaTurmaQuery : IRequest<IEnumerable<Inscricao>>
    {
        public ObtertInscricoesPorPropostaTurmaQuery(long turmaId)
        {
            TurmaId = turmaId;
        }

        public long TurmaId { get; set; }
    }

    public class ObtertInscricoesPorPropostaTurmaQueryValidator : AbstractValidator<ObtertInscricoesPorPropostaTurmaQuery>
    {
        public ObtertInscricoesPorPropostaTurmaQueryValidator()
        {
            RuleFor(x => x.TurmaId).GreaterThan(0).WithMessage("Informe o Id da Turma para obter as incrições");
        }
    }
}