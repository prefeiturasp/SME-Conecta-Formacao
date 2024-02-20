using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaEncontroCommand : IRequest<long>
    {
        public SalvarPropostaEncontroCommand(long propostaId, PropostaEncontroDTO encontroDTO)
        {
            PropostaId = propostaId;
            EncontroDTO = encontroDTO;
        }

        public long PropostaId { get; }
        public PropostaEncontroDTO EncontroDTO { get; }
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
