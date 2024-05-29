using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCancelarInscricaoCommand : IRequest<bool>
    {
        public EnviarEmailCancelarInscricaoCommand(long inscricaoId, string motivo)
        {
            InscricaoId = inscricaoId;
            Motivo = motivo;
        }

        public long InscricaoId { get; set; }
        public string Motivo { get; set; }
    }

    public class EnviarEmailCancelarInscricaoCommandValidator : AbstractValidator<EnviarEmailCancelarInscricaoCommand>
    {
        public EnviarEmailCancelarInscricaoCommandValidator()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0)
                .WithMessage("Informe o Id da Inscrição para Enviaro Email de Cancelamento");
            RuleFor(x => x.Motivo).NotEmpty()
                .WithMessage("Informe o motivo do cancelamento para Enviaro Email de Cancelamento");
        }
    }
}