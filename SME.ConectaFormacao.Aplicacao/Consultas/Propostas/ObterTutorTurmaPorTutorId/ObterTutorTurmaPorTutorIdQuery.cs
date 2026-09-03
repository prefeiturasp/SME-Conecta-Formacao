using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTutorTurmaPorTutorIdQuery : IRequest<IEnumerable<PropostaTutorTurma>>
    {
        public ObterTutorTurmaPorTutorIdQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class ObterTutorTurmaPorTutorIdQueryValidator : AbstractValidator<ObterTutorTurmaPorTutorIdQuery>
    {
        public ObterTutorTurmaPorTutorIdQueryValidator()
        {
            RuleFor(x => x.TutorId).GreaterThan(0).WithMessage("Informe o Id do Tutor");
        }
    }
}