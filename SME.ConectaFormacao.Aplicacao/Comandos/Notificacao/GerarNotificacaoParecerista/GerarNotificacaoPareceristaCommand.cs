using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoPareceristaCommand : IRequest<bool>
    {
        public GerarNotificacaoPareceristaCommand(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            Proposta = proposta;
            Pareceristas = pareceristas;
        }

        public Proposta Proposta { get; }
        public IEnumerable<PropostaPareceristaResumidoDTO> Pareceristas { get; }
    }

    public class GerarNotificacaoPareceristaCommandValidator : AbstractValidator<GerarNotificacaoPareceristaCommand>
    {
        public GerarNotificacaoPareceristaCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação aos pareceristas");
            
            RuleFor(t => t.Pareceristas)
                .NotEmpty()
                .WithMessage("É necessário informar os logins dos parecristas para gerar a notificação aos pareceristas");
        }
    }
}
