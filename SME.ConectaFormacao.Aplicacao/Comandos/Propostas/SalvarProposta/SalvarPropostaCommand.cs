using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaCommand : IRequest<bool>
    {
        public SalvarPropostaCommand(long propostaId, Proposta proposta, long? arquivoImagemDivulgacaoId)
        {
            PropostaId = propostaId;
            Proposta = proposta;
            ArquivoImagemDivulgacaoId = arquivoImagemDivulgacaoId;
        }

        public long PropostaId { get; }
        public Proposta Proposta { get; }

        public long? ArquivoImagemDivulgacaoId { get; }
    }

    public class SalvarPropostaCommandValidator : AbstractValidator<SalvarPropostaCommand>
    {
        public SalvarPropostaCommandValidator()
        {
        }
    }
}
