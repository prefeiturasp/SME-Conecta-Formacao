using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoDaPropostaCommand : IRequest<bool>
    {
        public AlterarSituacaoDaPropostaCommand(long id, SituacaoProposta situacaoProposta)
        {
            Id = id;
            SituacaoProposta = situacaoProposta;
        }

        public long Id { get; set; }

        public SituacaoProposta SituacaoProposta { get; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoDaPropostaCommandValidator : AbstractValidator<AlterarSituacaoDaPropostaCommand>
    {
        public AlterarSituacaoDaPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id para alteração da situação da proposta");

            RuleFor(f => f.SituacaoProposta)
                .NotNull()
                .WithMessage("É necessário informar a situação para alteração da situação da proposta");
        }
    }
}

