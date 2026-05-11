using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class DadosProcessamentoCertificadoCodafDto
    {
        public long Id { get; set; }
        public long CodigoCertificado { get; set; }
        public string HtmlContentSnapshot { get; set; } = null!;
        public string NomeCompleto { get; set; } = null!;
        public string EmailUsuario { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
        public bool TemRf { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; set; }
        public string SiglaCoordenadoriaOuDre { get; set; } = null!;
    }
}
