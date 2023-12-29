using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorIdQuery : IRequest<PropostaTurma>
    {
        public ObterPropostaTurmaPorIdQuery(long propostaTurmaId)
        {
            PropostaTurmaId = propostaTurmaId;
        }

        public long PropostaTurmaId { get; }
    }

    public class ObterPropostaTurmaPorIdQueryValidator : AbstractValidator<ObterPropostaTurmaPorIdQuery>
    {
        public ObterPropostaTurmaPorIdQueryValidator()
        {
            RuleFor(r => r.PropostaTurmaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter a proposta turma");
        }
    }
}
