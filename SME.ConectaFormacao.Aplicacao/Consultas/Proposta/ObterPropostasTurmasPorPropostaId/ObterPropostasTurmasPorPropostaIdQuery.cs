using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasTurmasPorPropostaIdQuery : IRequest<IEnumerable<long>>
    {
        public ObterPropostasTurmasPorPropostaIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class
        ObterPropostasTurmasPorPropostaIdQueryValidator : AbstractValidator<ObterPropostasTurmasPorPropostaIdQuery>
    {
        public ObterPropostasTurmasPorPropostaIdQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informa o ID da Proposta para Obter as turmas");
        }
    }
}