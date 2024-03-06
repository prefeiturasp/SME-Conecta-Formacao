using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraCompletaPorIdQuery : IRequest<AreaPromotoraCompletoDTO>
    {
        public ObterAreaPromotoraCompletaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterAreaPromotoraCompletaPorIdQueryValidator : AbstractValidator<ObterAreaPromotoraCompletaPorIdQuery>
    {
        public ObterAreaPromotoraCompletaPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da área promotora para obter os dados completos");
        }
    }
}
