using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class DadosProcessamentoCodafDto
    {
        public long Id { get; set; }
        public long CodigoDeclaracaoOuCertificado { get; set; }
        public string HtmlContentSnapshot { get; set; } = null!;
        public string NomeCompleto { get; set; } = null!;
        public string? NomeSocial { get; set; }
        public string NomeExibicao => string.IsNullOrWhiteSpace(NomeSocial) ? NomeCompleto : NomeSocial;
        public string EmailUsuario { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
        public bool TemRf { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; set; }
        public string Emissor { get; set; } = null!;
    }
}
