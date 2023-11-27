using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaMovimentacaoCommand : IRequest<bool>
    {
        public SalvarPropostaMovimentacaoCommand(long propostaId, SituacaoProposta situacao, string justificativa = null)
        {
            PropostaId = propostaId;
            Situacao = situacao;
            Justificativa = justificativa;
        }

        public long PropostaId { get; }

        public SituacaoProposta Situacao { get; }

        public string Justificativa { get; }
    }

    public class SalvarPropostaMovimentacaoCommandValidator : AbstractValidator<SalvarPropostaMovimentacaoCommand>
    {
        public SalvarPropostaMovimentacaoCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para salvar o parecer da proposta");

            RuleFor(x => x.Situacao)
                .NotNull()
                .NotEmpty()
                .WithMessage("Informe a situação para salvar o parecer da proposta");
        }
    }
}