using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPorIdQuery(long id) : IRequest<Proposta>
    {
        public long Id { get; } = id;
    }
    public class ObterPropostaPorIdQueryValidator : AbstractValidator<ObterPropostaPorIdQuery>
    {
        public ObterPropostaPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter a proposta");
        }
    }
}