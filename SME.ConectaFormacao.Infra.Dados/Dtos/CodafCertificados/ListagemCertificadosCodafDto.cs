using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados
{
    public class ListagemCertificadosCodafDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public string Documento { get; set; } = null!;
        public long CodigoCertificado { get; set; }
        public TipoCertificadoCodaf TipoCertificado { get; set; }
        public DateTime DataEmissao { get; set; }
        public string? NomeParticipante { get; set; }
    }
}