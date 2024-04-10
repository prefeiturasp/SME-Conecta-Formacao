namespace SME.ConectaFormacao.Aplicacao.Dtos.Email
{
    public class EnviarEmailDto
    {
        public string NomeDestinatario { get; set; } = string.Empty;
        public string EmailDestinatario { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string MensagemHtml { get; set; } = string.Empty;
    }
}