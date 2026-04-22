using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaEncontro
{
    public class SalvarPropostaEncontroCommand(long propostaId, PropostaEncontroDto encontroDto) : IRequest<long>
    {
        public long PropostaId { get; } = propostaId;
        public PropostaEncontroDto EncontroDto { get; } = encontroDto;
    }

    public class SalvarPropostaEncontroCommandValidator : AbstractValidator<SalvarPropostaEncontroCommand>
    {
        public SalvarPropostaEncontroCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar os encontros");
        }
    }
}
