namespace SME.ConectaFormacao.Infra.Servicos.Acessos.Options
{
    public class ServicoAcessosOptions
    {
        public const string Secao = "ServicoAcessos";

        public string UrlApi { get; set; }
        public string KeyApi { get; set; }
        public bool CodigoSistema { get; set; }
    }
}
