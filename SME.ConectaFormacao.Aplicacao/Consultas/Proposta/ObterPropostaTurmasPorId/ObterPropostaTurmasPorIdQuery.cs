using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasPorIdQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterPropostaTurmasPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterPropostaTurmasPorIdQueryValidator : AbstractValidator<ObterPropostaTurmasPorIdQuery>
    {
        public ObterPropostaTurmasPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
             .NotEmpty()
             .WithMessage("É nescessário informar o id para obter as turmas da proposta");
        }
    }
}
