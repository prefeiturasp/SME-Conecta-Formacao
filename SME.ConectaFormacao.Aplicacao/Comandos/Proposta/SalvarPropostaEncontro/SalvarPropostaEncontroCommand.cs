using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaEncontroCommand : IRequest<bool>
    {
        public SalvarPropostaEncontroCommand(long propostaId, IEnumerable<PropostaEncontro> encontros)
        {
            PropostaId = propostaId;
            Encontros = encontros;
        }

        public long PropostaId { get; }
        public IEnumerable<PropostaEncontro> Encontros { get; }
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
