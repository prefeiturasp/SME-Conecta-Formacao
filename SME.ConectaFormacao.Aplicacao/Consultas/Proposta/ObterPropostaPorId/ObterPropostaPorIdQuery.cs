using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorIdQuery : IRequest<PropostaCompletoDTO>
    {
        public ObterPropostaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterPropostaPorIdQueryValidator : AbstractValidator<ObterPropostaPorIdQuery>
    {
        public ObterPropostaPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É nescessário informar o id para obter a proposta");
        }
    }
}
