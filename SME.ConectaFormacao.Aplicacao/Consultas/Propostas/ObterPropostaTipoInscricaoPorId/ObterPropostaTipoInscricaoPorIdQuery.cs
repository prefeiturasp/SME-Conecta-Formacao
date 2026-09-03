using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
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
