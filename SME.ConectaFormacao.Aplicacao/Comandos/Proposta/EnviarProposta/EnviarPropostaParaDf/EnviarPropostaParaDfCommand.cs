using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarPropostaParaDfCommand : IRequest
    {
        public EnviarPropostaParaDfCommand(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class EnviarPropostaParaDfCommandValidator : AbstractValidator<EnviarPropostaParaDfCommand>
    {
        public EnviarPropostaParaDfCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta para enviar para o DF");
        }
    }
}