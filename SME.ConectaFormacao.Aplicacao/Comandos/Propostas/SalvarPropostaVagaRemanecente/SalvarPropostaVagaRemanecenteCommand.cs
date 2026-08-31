using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaVagaRemanecenteCommand : IRequest<bool>
    {
        public SalvarPropostaVagaRemanecenteCommand(long propostaId, IEnumerable<PropostaVagaRemanecente> vagasRemanecentes)
        {
            PropostaId = propostaId;
            VagasRemanecentes = vagasRemanecentes;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaVagaRemanecente> VagasRemanecentes { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class SalvarPropostaVagaRemanecenteCommandValidator : AbstractValidator<SalvarPropostaVagaRemanecenteCommand>
    {
        public SalvarPropostaVagaRemanecenteCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar as vagas remanecentes");
        }
    }
}
