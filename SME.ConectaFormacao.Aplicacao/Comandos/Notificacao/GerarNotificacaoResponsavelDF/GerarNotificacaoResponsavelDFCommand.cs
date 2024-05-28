using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoResponsavelDFCommand : IRequest<bool>
    {
        public GerarNotificacaoResponsavelDFCommand(Proposta proposta,PropostaPareceristaResumidoDTO parecerista)
        {
            Proposta = proposta;
            Parecerista = parecerista;
        }

        public Proposta Proposta { get; }
        public PropostaPareceristaResumidoDTO Parecerista { get; }
    }

    public class GerarNotificacaoResponsavelDFCommandValidator : AbstractValidator<GerarNotificacaoResponsavelDFCommand>
    {
        public GerarNotificacaoResponsavelDFCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação de reanálise do parecerista");
            
            RuleFor(t => t.Parecerista)
                .NotEmpty()
                .WithMessage("É necessário informar os pareceristas para gerar a notificação de reanálise do parecerista");
        }
    }
}

