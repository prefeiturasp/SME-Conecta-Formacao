using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Interface;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class DadosEmissaoDeclaracaoCodafDto : ICargaHoraria
    {
        public long IdReferencia { get; set; }
        public long InscricaoId { get; set; }
        public long PropostaTurmaId { get; set; }
        public required string NomeCompleto { get; set; }
        public string? NomeSocial { get; set; }
        public string NomeExibicao => string.IsNullOrWhiteSpace(NomeSocial) ? NomeCompleto : NomeSocial;
        public required string Documento { get; set; }
        public bool TemRf { get; set; }
        public TipoParticipacaoCodaf TipoParticipacao { get; set; }
        public required string NomeFormacao { get; set; }
        public DateTime DataRealizacao { get; set; }
        public int? HorasTotais { get; set; }
        public string? CargaHorariaTotalOutra { get; set; }
        public DateTime DataPublicacao { get; set; }
        public string? EmailUsuario { get; set; }
        public string Emissor { get; set; } = string.Empty;
        public string? EmissorSigla { get; set; }
        public TipoEmissor TipoEmissor { get; set; }
        public string TipoFormacao { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }
}
