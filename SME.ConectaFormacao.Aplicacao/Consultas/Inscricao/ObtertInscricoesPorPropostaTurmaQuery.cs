using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra;


namespace SME.ConectaFormacao.Aplicacao
{
    public class ObtertInscricoesPorPropostaTurmaQuery : IRequest<IEnumerable<InscricaoUsuarioInternoDto>>
    {
        public ObtertInscricoesPorPropostaTurmaQuery(long[] turmasIds)
        {
            TurmasIds = turmasIds;
        }

        public long[] TurmasIds { get; set; }
    }

    public class ObtertInscricoesPorPropostaTurmaQueryValidator : AbstractValidator<ObtertInscricoesPorPropostaTurmaQuery>
    {
        public ObtertInscricoesPorPropostaTurmaQueryValidator()
        {
            RuleFor(x => x.TurmasIds).NotEmpty().WithMessage("Informe o Id da Turma para obter as incrições");
        }
    }
}