using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTipoInscricaoPorIdQuery : IRequest<IEnumerable<PropostaTipoInscricao>>
    {
        public ObterPropostaTipoInscricaoPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
    }

    public class ObterPropostaTipoInscricaoPorIdQueryValidator : AbstractValidator<ObterPropostasPorIdsQuery>
    {
        public ObterPropostaTipoInscricaoPorIdQueryValidator()
        {
            RuleFor(x => x.PropostasIds)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter os tipos de inscrição");
        }
    }
}
