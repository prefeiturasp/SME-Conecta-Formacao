using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorIdQuery : IRequest<AreaPromotora>
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
