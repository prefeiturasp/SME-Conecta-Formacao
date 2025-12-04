using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao
{
    public class
        EnviarEmailConfirmacaoInscricaoCommandValidador : AbstractValidator<EnviarEmailConfirmacaoInscricaoCommand>
    {
        public EnviarEmailConfirmacaoInscricaoCommandValidador()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0).WithMessage("Informe o Id da Inscrição para realizar o envio do e-mail de confirmação");
        }
    }
}