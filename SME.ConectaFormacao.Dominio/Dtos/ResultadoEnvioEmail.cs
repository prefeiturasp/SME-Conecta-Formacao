namespace SME.ConectaFormacao.Dominio.Dtos
{
    public class ResultadoEnvioEmail
    {
        public bool Enviado { get; set; }
        public bool JaEnviado { get; set; }
        public string? MensagemErro { get; set; }
        public string? ChaveIdempotencia { get; set; }
        public DateTime DataProcessamento { get; set; }
        public static ResultadoEnvioEmail Sucesso(string? chaveIdempotencia = null)
        {
            return new ResultadoEnvioEmail
            {
                Enviado = true,
                JaEnviado = false,
                ChaveIdempotencia = chaveIdempotencia,
                DataProcessamento = DateTime.Now
            };
        }
        public static ResultadoEnvioEmail JaEnviadoAnteriormente(string chaveIdempotencia)
        {
            return new ResultadoEnvioEmail
            {
                Enviado = false,
                JaEnviado = true,
                ChaveIdempotencia = chaveIdempotencia,
                DataProcessamento = DateTime.Now
            };
        }
        public static ResultadoEnvioEmail Erro(string mensagemErro, string? chaveIdempotencia = null)
        {
            return new ResultadoEnvioEmail
            {
                Enviado = false,
                JaEnviado = false,
                MensagemErro = mensagemErro,
                ChaveIdempotencia = chaveIdempotencia,
                DataProcessamento = DateTime.Now
            };
        }
    }
}
