using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterTutorTurmaPorTutorId
{
    public class ObterTutorTurmaPorTutorIdQuery : IRequest<IEnumerable<PropostaTutorTurma>>
    {
        public ObterTutorTurmaPorTutorIdQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get; set; }
    }
    public class ObterTutorTurmaPorTutorIdQueryValidator : AbstractValidator<ObterTutorTurmaPorTutorIdQuery>
    {
        public ObterTutorTurmaPorTutorIdQueryValidator()
        {
            RuleFor(x =>x.TutorId).GreaterThan(0).WithMessage("Informe o Id do Tutor");
        }
    }
}