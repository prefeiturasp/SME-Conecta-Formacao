using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarPropostaRascunhoCommand : IRequest<RetornoDTO>
    {
        public AlterarPropostaRascunhoCommand(long id, PropostaDTO propostaDTO)
        {
            Id = id;
            PropostaDTO = propostaDTO;
        }

        public long Id { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    [ExcludeFromCodeCoverage]
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

