using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRelatorioProspostaLaudaPublicacaoQuery : IRequest<string>
    {
        public ObterRelatorioProspostaLaudaPublicacaoQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ObterRelatorioProspostaLaudaPublicacaoQueryValidator : AbstractValidator<ObterRelatorioProspostaLaudaPublicacaoQuery>
    {
        public ObterRelatorioProspostaLaudaPublicacaoQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para o relatório de proposta lauda de publicação");
        }
    }
}
