using SME.ConectaFormacao.Dominio.Enumerados;
using System.Text.Json;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCertificado : EntidadeBaseAuditavel
    {
        public long CodigoCertificado { get; init; }
        public long CodafInscricaoListaPresencaId { get; private set; }
        public CodafInscricaoListaPresenca? CodafInscricaoListaPresenca { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public string HtmlContentSnapshot { get; private set; }
        public string? MetadadosJson { get; private set; }

        protected CodafCertificado()
        {
            HtmlContentSnapshot = null!;
        }
        public CodafCertificado(long codafInscricaoListaPresencaId, TipoParticipacaoCodaf tipoParticipacao, string htmlContentSnapshot, object metadadosJson)
        {
            if (string.IsNullOrWhiteSpace(htmlContentSnapshot))
                throw new ArgumentException("O snapshot do HTML do certificado é obrigatório.", nameof(htmlContentSnapshot));
            if (codafInscricaoListaPresencaId <= 0)
                throw new ArgumentException("ID de inscrição inválido.", nameof(codafInscricaoListaPresencaId));
            CodafInscricaoListaPresencaId = codafInscricaoListaPresencaId;
            TipoParticipacao = tipoParticipacao;
            DataEmissao = DateTime.Now;
            HtmlContentSnapshot = htmlContentSnapshot;
            if (metadadosJson != null)
                MetadadosJson = JsonSerializer.Serialize(metadadosJson);
        }
    }
}
