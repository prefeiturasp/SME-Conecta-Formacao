using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraCommand : IRequest<bool>
    {
        public GerarNotificacaoAreaPromotoraCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; }
    }

    public class GerarNotificacaoAreaPromotoraCommandValidator : AbstractValidator<GerarNotificacaoAreaPromotoraCommand>
    {
        public GerarNotificacaoAreaPromotoraCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação a Área Promotora");
        }
    }
}

