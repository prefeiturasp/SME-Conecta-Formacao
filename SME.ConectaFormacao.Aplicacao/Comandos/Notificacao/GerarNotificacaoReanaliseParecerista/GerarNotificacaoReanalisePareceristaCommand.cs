using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoReanalisePareceristaCommand : IRequest<bool>
    {
        public GerarNotificacaoReanalisePareceristaCommand(Proposta proposta,IEnumerable<PropostaPareceristaResumidoDTO> pareceristas)
        {
            Proposta = proposta;
            Pareceristas = pareceristas;
        }

        public Proposta Proposta { get; }
        public IEnumerable<PropostaPareceristaResumidoDTO> Pareceristas { get; }
    }

    public class GerarNotificacaoReanalisePareceristaCommandValidator : AbstractValidator<GerarNotificacaoReanalisePareceristaCommand>
    {
        public GerarNotificacaoReanalisePareceristaCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação de reanálise do parecerista");
            
            RuleFor(t => t.Pareceristas)
                .NotEmpty()
                .WithMessage("É necessário informar os pareceristas para gerar a notificação de reanálise do parecerista");
        }
    }
}

