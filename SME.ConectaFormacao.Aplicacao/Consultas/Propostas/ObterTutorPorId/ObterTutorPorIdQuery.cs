using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTutorPorIdQuery : IRequest<PropostaTutor>
    {
        public ObterTutorPorIdQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class ObterTutorPorIdQueryValidator : AbstractValidator<ObterTutorPorIdQuery>
    {
        public ObterTutorPorIdQueryValidator()
        {
            RuleFor(x => x.TutorId).GreaterThan(0).WithMessage("Informe o Id do Tutor");
        }
    }
}