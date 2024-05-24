using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUltimoParecerPropostaQuery : IRequest<PropostaMovimentacaoDTO>
    {
        public ObterUltimoParecerPropostaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
    }

    public class ObterUltimoParecerPropostaQueryValidator : AbstractValidator<ObterUltimoParecerPropostaQuery>
    {
        public ObterUltimoParecerPropostaQueryValidator()
        {
            RuleFor(t => t.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o id da proposta para obter o ultimo parecer");
        }
    }
}
