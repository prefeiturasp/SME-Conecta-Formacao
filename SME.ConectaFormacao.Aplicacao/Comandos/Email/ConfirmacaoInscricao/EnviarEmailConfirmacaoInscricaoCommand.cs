using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailConfirmacaoInscricaoCommand : IRequest<bool>
    {
        public EnviarEmailConfirmacaoInscricaoCommand(long inscricaoId)
        {
            InscricaoId = inscricaoId;
        }

        public long InscricaoId { get; set; }
    }

    public class
        EnviarEmailConfirmacaoInscricaoCommandValidador : AbstractValidator<EnviarEmailConfirmacaoInscricaoCommand>
    {
        public EnviarEmailConfirmacaoInscricaoCommandValidador()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0).WithMessage("Informe o Id da Inscrição para realizar o envio do e-mail de confirmação");
        }
    }
}