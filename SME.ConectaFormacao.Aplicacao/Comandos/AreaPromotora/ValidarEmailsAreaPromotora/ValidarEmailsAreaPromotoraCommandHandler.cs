using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEmailsAreaPromotoraCommandHandler : IRequestHandler<ValidarEmailsAreaPromotoraCommand>
    {
        public Task Handle(ValidarEmailsAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Email))
            {
                var mensagens = new List<string>();

                var emails = request.Email.Split(';');
                foreach (var email in emails)
                {
                    if (!email.EmailEhValido())
                        mensagens.Add(string.Format(MensagemNegocio.EMAIL_INVALIDO, email));
                    else if (request.Tipo == AreaPromotoraTipo.RedeDireta && !email.ToLower().Contains("@sme") && !email.ToLower().Contains("@edu.sme"))
                        throw new NegocioException(MensagemNegocio.AREA_CONHECIMENTO_EMAIL_FORA_DOMINIO_REDE_DIRETA);
                }

                if (mensagens.Any())
                    throw new NegocioException(mensagens);
            }

            return Task.CompletedTask;
        }
    }
}
