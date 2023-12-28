using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmasComVagasPorIdQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterPropostaTurmasComVagasPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
    }

    public class ObterPropostaTurmasComVagasPorIdQueryValidator : AbstractValidator<ObterPropostaTurmasComVagasPorIdQuery>
    {
        public ObterPropostaTurmasComVagasPorIdQueryValidator()
        {
            RuleFor(t => t.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter as turmas com vaga disponível");
        }
    }
}
