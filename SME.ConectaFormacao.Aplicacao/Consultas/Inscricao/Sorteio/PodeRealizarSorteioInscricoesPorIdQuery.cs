using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PodeRealizarSorteioInscricoesPorIdQuery : IRequest<bool>
    {
        public PodeRealizarSorteioInscricoesPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class PodeRealizarSorteioInscricoesPorIdQueryValidator : AbstractValidator<PodeRealizarSorteioInscricoesPorIdQuery>
    {
        public PodeRealizarSorteioInscricoesPorIdQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da proposta para verificar se pode realizar sorteio das inscrições");
        }
    }
}