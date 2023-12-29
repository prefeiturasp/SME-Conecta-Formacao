using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaDresPorPropostaTurmaIdQuery : IRequest<IEnumerable<PropostaTurmaDre>>
    {
        public ObterPropostaTurmaDresPorPropostaTurmaIdQuery(params long[] propostaTurmaIds)
        {
            PropostaTurmaIds = propostaTurmaIds;
        }

        public long[] PropostaTurmaIds { get; }
    }

    public class ObterPropostaTurmaDresPorPropostaTurmaIdQueryValidator : AbstractValidator<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>
    {
        public ObterPropostaTurmaDresPorPropostaTurmaIdQueryValidator()
        {
            RuleFor(r => r.PropostaTurmaIds)
                .NotEmpty()
                .WithMessage("É necessário informar ao menos um id de proposta turma para obter as dres");
        }
    }
}
