using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterPropostaTurmasComVagasPorIdQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterPropostaTurmasComVagasPorIdQuery(long propostaId, string? codigoDre = null)
        {
            PropostaId = propostaId;
            CodigoDre = codigoDre;
        }

        public long PropostaId { get; }
        public string? CodigoDre { get; set; }

        [ExcludeFromCodeCoverage]
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
}
