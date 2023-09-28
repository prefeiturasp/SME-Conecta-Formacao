using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaRascunhoCommand : IRequest<long>
    {
        public AlterarPropostaRascunhoCommand(long id, PropostaDTO propostaDTO)
        {
            Id = id;
            PropostaDTO = propostaDTO;
        }

        public long Id { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class AlterarPropostaRascunhoCommandValidator : AbstractValidator<AlterarPropostaRascunhoCommand>
    {
        public AlterarPropostaRascunhoCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id para alterar a proposta");
        }
    }
}
