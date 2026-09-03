using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class EnviarEmailConfirmacaoInscricaoCommandValidador : AbstractValidator<EnviarEmailConfirmacaoInscricaoCommand>
    {
        public EnviarEmailConfirmacaoInscricaoCommandValidador()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0).WithMessage("Informe o Id da Inscrição para realizar o envio do e-mail de confirmação");
        }
    }
}