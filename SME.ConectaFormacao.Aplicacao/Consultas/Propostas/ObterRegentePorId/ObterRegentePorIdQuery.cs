using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentePorId
{
    [ExcludeFromCodeCoverage]
    public class ObterRegentePorIdQuery : IRequest<PropostaRegente>
    {
        public ObterRegentePorIdQuery(long regenteId)
        {
            RegenteId = regenteId;
        }

        public long RegenteId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterRegentePorIdQueryValidator : AbstractValidator<ObterRegentePorIdQuery>
    {
        public ObterRegentePorIdQueryValidator()
        {
            RuleFor(x => x.RegenteId).GreaterThan(0).WithMessage("Informe o Id do Regente");
        }
    }
}