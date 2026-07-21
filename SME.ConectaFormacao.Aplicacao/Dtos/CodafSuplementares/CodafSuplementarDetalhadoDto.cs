using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares
{
    public class CodafSuplementarDetalhadoDto
    {
        public long Id { get; set; }
        public long CodafId { get; set; }
        public long PropostaId { get; set; }
        public long PropostaTurmaId { get; set; }
        public DateTime? DataPublicacao { get; set; }
        public DateTime? DataPublicacaoDom { get; set; }
        public short? NumeroComunicado { get; set; }
        public short? PaginaComunicadoDom { get; set; }
        public int? CodigoCursoEol { get; set; }
        public int? CodigoNivel { get; set; }
        public string? Observacao { get; set; }
        public StatusCodafSuplementar Status { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? AlteradoLogin { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? CriadoPor { get; set; }
        public string? CriadoLogin { get; set; }
        public string? NomeFormacao { get; set; }
        public long? CodigoFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public bool CertificadoEmitido { get; set; }
        public IList<CodafSuplementarRetificacaoDto>? Retificacoes { get; set; }
        public IList<CodafSuplementarAnexoDto>? Anexos { get; set; }
        public IList<CodafCertificadoDto>? Certificados { get; set; }
        public IList<CodafSuplementarInscritoDto>? Inscritos { get; set; }
        public RegrasAprovacaoCursistaDto? RegrasAprovacao { get; set; }
    }
}
