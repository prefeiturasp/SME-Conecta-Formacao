using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterTutorPorId
{
    public class ObterTutorPorIdQuery : IRequest<PropostaTutor>
    {
        public ObterTutorPorIdQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
    public class ObterTutorPorIdQueryValidator : AbstractValidator<ObterTutorPorIdQuery>
    {
        public ObterTutorPorIdQueryValidator()
        {
            RuleFor(x => x.TutorId).GreaterThan(0).WithMessage("Informe o Id do Tutor");
        }
    }
}