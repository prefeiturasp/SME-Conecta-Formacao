using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRelatorioProspostaLaudaCompletaQuery : IRequest<string>
    {
        public ObterRelatorioProspostaLaudaCompletaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class ObterRelatorioProspostaLaudaCompletaQueryValidator : AbstractValidator<ObterRelatorioProspostaLaudaCompletaQuery>
    {
        public ObterRelatorioProspostaLaudaCompletaQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para o relatório de proposta lauda de publicação");
        }
    }
}
