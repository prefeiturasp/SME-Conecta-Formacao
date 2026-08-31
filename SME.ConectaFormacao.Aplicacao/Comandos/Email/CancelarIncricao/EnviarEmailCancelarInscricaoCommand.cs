using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCancelarInscricaoCommand : IRequest<bool>
    {
        public EnviarEmailCancelarInscricaoCommand(long inscricaoId, string? motivo)
        {
            InscricaoId = inscricaoId;
            Motivo = motivo;
        }

        public long InscricaoId { get; set; }
        public string? Motivo { get; set; }
    }


    [ExcludeFromCodeCoverage]
    public class EnviarEmailCancelarInscricaoCommandValidator : AbstractValidator<EnviarEmailCancelarInscricaoCommand>
    {
        public EnviarEmailCancelarInscricaoCommandValidator()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0)
                .WithMessage("Informe o Id da Inscrição para Enviar o Email de Cancelamento");
        }
    }
}