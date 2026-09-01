using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegenteTurmaPorRegenteId
{
    public class ObterRegenteTurmaPorRegenteIdQuery : IRequest<IEnumerable<PropostaRegenteTurma>>
    {
        public ObterRegenteTurmaPorRegenteIdQuery(long regenteId)
        {
            RegenteId = regenteId;
        }

        public long RegenteId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterRegenteTurmaPorRegenteIdQueryValidator : AbstractValidator<ObterRegenteTurmaPorRegenteIdQuery>
    {
        public ObterRegenteTurmaPorRegenteIdQueryValidator()
        {
            RuleFor(x => x.RegenteId).GreaterThan(0).WithMessage("Informe o Id do Regente");
        }
    }
}