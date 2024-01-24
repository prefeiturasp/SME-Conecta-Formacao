using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaInscricaoAutomaticaPorIdQuery : IRequest<PropostaInscricaoAutomatica>
    {
        public ObterPropostaInscricaoAutomaticaPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }
    public class ObterPropostaInscricaoAutomaticaPorIdQueryValidator : AbstractValidator<ObterPropostaInscricaoAutomaticaPorIdQuery>
    {
        public ObterPropostaInscricaoAutomaticaPorIdQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter a proposta resumida.");
        }
    }
}