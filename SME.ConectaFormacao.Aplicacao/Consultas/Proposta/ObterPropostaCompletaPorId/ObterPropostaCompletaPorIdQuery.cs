using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaCompletaPorIdQuery : IRequest<PropostaCompletoDTO>
    {
        public ObterPropostaCompletaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterPropostaCompletaPorIdQueryValidator : AbstractValidator<ObterPropostaCompletaPorIdQuery>
    {
        public ObterPropostaCompletaPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter a proposta");
        }
    }
}
