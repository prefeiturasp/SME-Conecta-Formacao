using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoDFCommand : IRequest<bool>
    {
        public GerarNotificacaoDFCommand(Proposta proposta, PropostaPareceristaResumidoDTO parecerista)
        {
            Proposta = proposta;
            Parecerista = parecerista;
        }

        public Proposta Proposta { get; }
        public PropostaPareceristaResumidoDTO Parecerista { get; }
    }

    public class GerarNotificacaoDFCommandValidator : AbstractValidator<GerarNotificacaoDFCommand>
    {
        public GerarNotificacaoDFCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação ao Admin DF");
            
            RuleFor(t => t.Parecerista)
                .NotEmpty()
                .WithMessage("É necessário informar o login do pareceristas para gerar a notificação ao Admin DF");
        }
    }
}