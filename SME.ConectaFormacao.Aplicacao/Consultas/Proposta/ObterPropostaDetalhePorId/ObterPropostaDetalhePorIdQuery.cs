using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaDetalhePorIdQuery : IRequest<RetornoDetalheFormacaoDTO>
    {
        public ObterPropostaDetalhePorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
    public class ObterPropostaDetalhePorIdQueryValidator : AbstractValidator<ObterPropostaDetalhePorIdQuery>
    {
        public ObterPropostaDetalhePorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter o detalhe da proposta");
        }
    }
}