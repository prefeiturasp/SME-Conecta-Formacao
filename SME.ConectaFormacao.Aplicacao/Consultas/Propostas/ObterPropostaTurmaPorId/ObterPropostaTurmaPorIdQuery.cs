using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorIdQuery(long propostaTurmaId) : IRequest<PropostaTurma>
    {
        public long PropostaTurmaId { get; } = propostaTurmaId;
    }

    [ExcludeFromCodeCoverage]
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
