using SME.ConectaFormacao.Dominio.Enumerados;
using System.Text.Json;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CodafCertificado : EntidadeBaseAuditavel
    {
        public long? CodafListaPresencaId { get; set; }
        public long? CodafSuplementarId { get; set; }
        public long CodigoCertificado { get; init; }
        public long? CodafInscricaoListaPresencaId { get; private set; }
        public CodafInscricaoListaPresenca? CodafInscricaoListaPresenca { get; set; }
        public long? CodafSuplementarInscricaoId { get; set; }
        public CodafSuplementarInscricao? CodafSuplementarInscricao { get; set; }
        public long? PropostaRegenteTurmaId { get; private set; }
        public PropostaRegenteTurma? PropostaRegenteTurma { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public string HtmlContentSnapshot { get; private set; }
        public string? MetadadosJson { get; private set; }
        public StatusProcessamentoCertificadoCodaf StatusProcessamento { get; set; }
        public string? ChaveObjetoArmazenamento { get; set; }
        public string? ErroProcessamento { get; set; }

        protected CodafCertificado()
        {
            HtmlContentSnapshot = null!;
        }
        public CodafCertificado(long codafId, TipoCodaf tipoCodaf, TipoParticipacaoCodaf tipoParticipacao, long idReferencia, string htmlContentSnapshot, object metadadosJson)
        {
            DefinirOrigemCodaf(tipoCodaf, codafId);
            if (string.IsNullOrWhiteSpace(htmlContentSnapshot))
                throw new ArgumentException("O snapshot do HTML do certificado é obrigatório.", nameof(htmlContentSnapshot));
            if (idReferencia <= 0)
                throw new ArgumentException("O ID de referência (Inscrição ou Regente) é inválido.", nameof(idReferencia));
            TipoParticipacao = tipoParticipacao;
            DefinirTipoIdReferencia(tipoCodaf, tipoParticipacao, idReferencia);
            DataEmissao = DateTime.Now;
            HtmlContentSnapshot = htmlContentSnapshot;
            if (metadadosJson != null)
                MetadadosJson = JsonSerializer.Serialize(metadadosJson);
        }
        private void DefinirOrigemCodaf(TipoCodaf tipoCodaf, long codafId)
        {
            switch (tipoCodaf)
            {
                case TipoCodaf.ListaPresenca:
                    CodafListaPresencaId = codafId;
                    break;
                case TipoCodaf.Suplementar:
                    CodafSuplementarId = codafId;
                    break;
                default:
                    throw new ArgumentException("Tipo de Codaf não suportado.", nameof(tipoCodaf));
            }
        }

        private void DefinirTipoIdReferencia(TipoCodaf tipoCodaf, TipoParticipacaoCodaf tipoParticipacao, long idReferencia)
        {
            switch (tipoParticipacao, tipoCodaf)
            {
                case (TipoParticipacaoCodaf.Cursista, TipoCodaf.Suplementar):
                    CodafSuplementarInscricaoId = idReferencia;
                    break;
                case (TipoParticipacaoCodaf.Cursista, _):
                    CodafInscricaoListaPresencaId = idReferencia;
                    break;
                case (TipoParticipacaoCodaf.Regente, _):
                    PropostaRegenteTurmaId = idReferencia;
                    break;
                default:
                    throw new ArgumentException("Tipo de participação não suportado para emissão de certificado.", nameof(tipoParticipacao));
            }
        }
    }
}
