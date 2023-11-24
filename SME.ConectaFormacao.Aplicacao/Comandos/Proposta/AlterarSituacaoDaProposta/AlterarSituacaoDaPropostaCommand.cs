using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
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
