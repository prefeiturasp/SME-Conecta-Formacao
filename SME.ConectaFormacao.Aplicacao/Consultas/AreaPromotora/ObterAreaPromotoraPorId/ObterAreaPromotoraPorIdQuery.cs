using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorIdQuery : IRequest<AreaPromotoraCompletoDTO>
    {
        public ObterAreaPromotoraPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterAreaPromotoraPorIdQueryValidator : AbstractValidator<ObterAreaPromotoraPorIdQuery>
    {
        public ObterAreaPromotoraPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id da área promotora para obter os dados");
        }
    }
}
