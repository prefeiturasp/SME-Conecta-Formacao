namespace SME.ConectaFormacao.Infra.Servicos.Acessos
{
    public class AcessosConfiguracaoEmailRetorno
    {
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Smtp { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public int Porta { get; set; }
        public bool TLS { get; set; }
    }
}