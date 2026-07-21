using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafCertificadoDto
    {
        public long? CodafListaPresencaId { get; set; }
        public long? CodafSuplementarId { get; set; }
        public long CodigoCertificado { get; init; }
        public long? CodafInscricaoListaPresencaId { get; private set; }
        public long? CodafSuplementarInscricaoId { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; private set; }
        public StatusProcessamentoCertificadoCodaf StatusProcessamento { get; set; }
    }
}
