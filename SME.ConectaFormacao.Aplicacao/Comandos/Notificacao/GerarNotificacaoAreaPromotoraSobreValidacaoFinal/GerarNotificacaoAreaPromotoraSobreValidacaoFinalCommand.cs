using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand : IRequest<bool>
    {
        public GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand(Proposta proposta)
        {
            Proposta = proposta;
        }

        public Proposta Proposta { get; }
    }

    public class GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandValidator : AbstractValidator<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>
    {
        public GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommandValidator()
        {
            RuleFor(t => t.Proposta)
                .NotEmpty()
                .WithMessage("É necessário informar a proposta para gerar a notificação a Área Promotora sobre validaçaõ final da DF");
        }
    }
}

