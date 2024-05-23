using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoCommand : IRequest<bool>
    {
        public GerarNotificacaoCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; }
    }

    public class GerarNotificacaoCommandValidator : AbstractValidator<GerarNotificacaoCommand>
    {
        public GerarNotificacaoCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação");
        }
    }
}
