namespace SME.ConectaFormacao.Aplicacao.Dtos.Email
{
    public class EnviarEmailDevolverPropostaDto
    {
        public string NomeDestinatario { get; set; } = string.Empty;
        public string EmailDestinatario { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;        
        public string Motivo { get; set; } = string.Empty;
    }
}