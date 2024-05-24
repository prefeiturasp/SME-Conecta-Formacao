using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoDFCommand : IRequest<bool>
    {
        public GerarNotificacaoDFCommand(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            Proposta = proposta;
            Pareceristas = pareceristas;
        }

        public Proposta Proposta { get; }
        public IEnumerable<PropostaPareceristaResumidoDTO> Pareceristas { get; }
    }

    public class GerarNotificacaoDFCommandValidator : AbstractValidator<GerarNotificacaoDFCommand>
    {
        public GerarNotificacaoDFCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação aos pareceristas");
            
            RuleFor(t => t.Pareceristas)
                .NotEmpty()
                .WithMessage("É necessário informar os logins dos pareceristas para gerar a notificação aos pareceristas");
        }
    }
}