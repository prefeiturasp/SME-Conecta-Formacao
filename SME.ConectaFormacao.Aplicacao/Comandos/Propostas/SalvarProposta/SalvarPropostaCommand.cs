using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

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

    [ExcludeFromCodeCoverage]
    public class SalvarPropostaCommandValidator : AbstractValidator<SalvarPropostaCommand>
    {
        public SalvarPropostaCommandValidator()
        {
            RuleFor(f => f.Proposta.SobreEsteCurso)
            .NotEmpty()
            .WithMessage("É necessário informar sobre este curso para inserir a proposta");
        }
    }
}
