using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public record ObterFormacaoDetalhadaPorIdQuery(long Id, FiltroListaFormacaoPropostaDto Filtro) : IRequest<RetornoFormacaoDetalhadaDTO>;

    public class ObterFormacaoDetalhadaPorIdQueryValidator : AbstractValidator<ObterFormacaoDetalhadaPorIdQuery>
    {
        public ObterFormacaoDetalhadaPorIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o id para obter o detalhamento da formação");
        }
    }
}