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
                .WithMessage("É necessário informar os ids das propostas para obter as informações da formação");
        }
    }
}
