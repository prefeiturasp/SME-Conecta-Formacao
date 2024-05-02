using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaCommand : IRequest<bool>
    {
        public SalvarPropostaPareceristaCommand(long propostaId,IEnumerable<PropostaParecerista> pareceristas)
        {
            PropostaId = propostaId;
            Pareceristas = pareceristas;
        }

        public long PropostaId { get; }

        public IEnumerable<PropostaParecerista> Pareceristas { get; }
    }

    public class SalvarPropostaPareceristaCommandValidator : AbstractValidator<SalvarPropostaPareceristaCommand>
    {
        public SalvarPropostaPareceristaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta Para salvar a proposta parecerista");
        }        
    }
}