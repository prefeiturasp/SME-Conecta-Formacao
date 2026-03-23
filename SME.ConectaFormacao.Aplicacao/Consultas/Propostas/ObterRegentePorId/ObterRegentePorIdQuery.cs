using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterRegentePorId
{
    public class ObterRegentePorIdQuery : IRequest<PropostaRegente>
    {
        public ObterRegentePorIdQuery(long regenteId)
        {
            RegenteId = regenteId;
        }

        public long RegenteId { get; set; }
    }

    public class ObterRegentePorIdQueryValidator : AbstractValidator<ObterRegentePorIdQuery>
    {
        public ObterRegentePorIdQueryValidator()
        {
            RuleFor(x => x.RegenteId).GreaterThan(0).WithMessage("Informe o Id do Regente");
        }
    }
}