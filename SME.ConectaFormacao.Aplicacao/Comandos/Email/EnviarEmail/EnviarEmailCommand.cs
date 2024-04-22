using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class EnviarEmailCommand : IRequest<bool>
    {
        public EnviarEmailCommand(string nomeDestinatario, string emailDestinatario, string assunto, string mensagemHtml)
        {
            NomeDestinatario = nomeDestinatario;
            EmailDestinatario = emailDestinatario;
            Assunto = assunto;
            MensagemHtml = mensagemHtml;
        }

        public string NomeDestinatario { get; }
        public string EmailDestinatario { get; }
        public string Assunto { get; }
        public string MensagemHtml { get; }
    }

    public class EnviarEmailCommandValidator : AbstractValidator<EnviarEmailCommand>
    {
        public EnviarEmailCommandValidator()
        {
            RuleFor(c => c.NomeDestinatario)
                .NotNull()
                .NotEmpty()
                .WithMessage("O nome do destinatário deve ser informado para o envio do e-mail.");

            RuleFor(c => c.EmailDestinatario)
                .NotNull()
                .NotEmpty()
                .WithMessage("O e-mail do destinatário deve ser informado para o envio do e-mail.")
                .EmailAddress()
                .WithMessage("O e-mail do destinatário não é válido.");

            RuleFor(c => c.Assunto)
                .NotNull()
                .NotEmpty()
                .WithMessage("O assunto deve ser informado para o envio do e-mail.");

            RuleFor(c => c.MensagemHtml)
                .NotNull()
                .NotEmpty()
                .WithMessage("A mensagem deve ser informada para o envio do e-mail.");
        }
    }
}