using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasPorIdsQuery : IRequest<IEnumerable<RetornoListagemFormacaoDTO>>
    {
        public ObterPropostasPorIdsQuery(IEnumerable<long> propostasIds)
        {
            PropostasIds = propostasIds;
        }

        public IEnumerable<long> PropostasIds { get; set; }
    }

    public class ObterPropostasPorIdsQueryValidator : AbstractValidator<ObterPropostasPorIdsQuery>
    {
        public ObterPropostasPorIdsQueryValidator()
        {
            RuleFor(x => x.PropostasIds)
                .NotEmpty()
                .WithMessage("Nenhuma proposta foi localizada com o filtro informado");
        }
    }
}
