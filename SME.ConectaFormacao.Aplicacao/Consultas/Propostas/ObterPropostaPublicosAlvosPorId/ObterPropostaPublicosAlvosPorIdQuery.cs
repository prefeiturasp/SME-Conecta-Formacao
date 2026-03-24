using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaPublicosAlvosPorIdQuery : IRequest<IEnumerable<PropostaPublicoAlvo>>
    {
        public ObterPropostaPublicosAlvosPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
    }

    public class ObterPropostaPublicosAlvosPorIdQueryValidator : AbstractValidator<ObterPropostaPublicosAlvosPorIdQuery>
    {
        public ObterPropostaPublicosAlvosPorIdQueryValidator()
        {
            RuleFor(r => r.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter os publico alvo");
        }
    }
}
