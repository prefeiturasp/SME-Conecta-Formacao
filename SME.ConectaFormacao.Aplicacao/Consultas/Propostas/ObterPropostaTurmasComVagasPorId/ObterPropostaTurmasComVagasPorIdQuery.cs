using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class ObterPropostaTurmasComVagasPorIdQuery : IRequest<IEnumerable<PropostaTurmaInscricaoDTO>>
    {
        public ObterPropostaTurmasComVagasPorIdQuery(long propostaId, string? codigoDre = null, bool apenasPeriodoInscricaoVigente = false)
        {
            PropostaId = propostaId;
            CodigoDre = codigoDre;
            ApenasPeriodoInscricaoVigente = apenasPeriodoInscricaoVigente;
        }

        public long PropostaId { get; }
        public string? CodigoDre { get; set; }
        public bool ApenasPeriodoInscricaoVigente { get; }

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
