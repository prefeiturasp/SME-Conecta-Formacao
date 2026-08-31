using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaPareceristaCommand : IRequest<bool>
    {
        public SalvarPropostaPareceristaCommand(long propostaId, IEnumerable<PropostaParecerista> pareceristas)
        {
            PropostaId = propostaId;
            Pareceristas = pareceristas;
        }

        public long PropostaId { get; }

        public IEnumerable<PropostaParecerista> Pareceristas { get; }
    }

    [ExcludeFromCodeCoverage]
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