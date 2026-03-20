using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarPropostaCommand : IRequest<bool>
    {
        public EnviarPropostaCommand(long propostaId, SituacaoProposta situacao)
        {
            PropostaId = propostaId;
            Situacao = situacao;
        }

        public long PropostaId { get; }

        public SituacaoProposta Situacao { get; }
    }

    public class EnviarPropostaCommandValidator : AbstractValidator<EnviarPropostaCommand>
    {
        public EnviarPropostaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para enviar para análise");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("Informe a situação da proposta para enviar para análise");
        }
    }
}