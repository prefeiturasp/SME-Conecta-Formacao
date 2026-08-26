using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;
using SME.ConectaFormacao.Dominio.Enumerados;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoGrupoGestaoDaPropostaCommand : IRequest<bool>
    {
        public AlterarSituacaoGrupoGestaoDaPropostaCommand(long id, SituacaoProposta situacaoProposta, long grupoGestaoId)
        {
            Id = id;
            SituacaoProposta = situacaoProposta;
            GrupoGestaoId = grupoGestaoId;
        }

        public long Id { get; set; }
        public long GrupoGestaoId { get; set; }

        public SituacaoProposta SituacaoProposta { get; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarSituacaoGrupoGestaoDaPropostaCommandValidator : AbstractValidator<AlterarSituacaoGrupoGestaoDaPropostaCommand>
    {
        public AlterarSituacaoGrupoGestaoDaPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id para alteração da situação da proposta");

            RuleFor(f => f.GrupoGestaoId)
                .GreaterThan(0)
                .WithMessage("É necessário informar o grupo gestão para alteração da situação da proposta");

            RuleFor(f => f.SituacaoProposta)
                .NotNull()
                .WithMessage("É necessário informar a situação para alteração da situação da proposta");
        }
    }
}

