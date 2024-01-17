using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaResumidaPorIdQuery : IRequest<IEnumerable<FormacaoResumida>>
    {
        public ObterPropostaResumidaPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }
        
        public long PropostaId { get; set; }
    }
    public class ObterPropostaPorTipoInscricaoESituacaoQueryValidator : AbstractValidator<ObterPropostaResumidaPorIdQuery>
    {
        public ObterPropostaPorTipoInscricaoESituacaoQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter a proposta resumida.");
        }
    }
}