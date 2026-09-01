using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterPropostaTurmasPorIdQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterPropostaTurmasPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterPropostaTurmasPorIdQueryValidator : AbstractValidator<ObterPropostaTurmasPorIdQuery>
    {
        public ObterPropostaTurmasPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
             .NotEmpty()
             .WithMessage("É necessário informar o id para obter as turmas da proposta");
        }
    }
}
