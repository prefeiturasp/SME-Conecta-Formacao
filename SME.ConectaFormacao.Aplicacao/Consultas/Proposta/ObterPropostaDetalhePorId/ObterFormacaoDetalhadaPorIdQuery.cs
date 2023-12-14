using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormacaoDetalhadaPorIdQuery : IRequest<RetornoFormacaoDetalhadaDTO>
    {
        public ObterFormacaoDetalhadaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
    public class ObterDetalheFormacaoPorIdQueryValidator : AbstractValidator<ObterFormacaoDetalhadaPorIdQuery>
    {
        public ObterDetalheFormacaoPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter o detalhe da formação");
        }
    }
}