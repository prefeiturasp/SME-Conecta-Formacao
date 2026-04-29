using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class FiltroListagemPropostaDto
    {
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
        public long? AreaPromotoraIdUsuarioLogado { get; set; }
        public long? PropostaId { get; set; }
        public long? AreaPromotoraId { get; set; }
        public Formato? Formato { get; set; }
        public long[]? PublicoAlvoIds { get; set; }
        public string? NomeFormacao { get; set; }
        public long? NumeroHomologacao { get; set; }
        public DateTime? PeriodoRealizacaoInicio { get; set; }
        public DateTime? PeriodoRealizacaoFim { get; set; }
        public SituacaoProposta? Situacao { get; set; }
        public bool? FormacaoHomologada { get; set; }
        public bool? Revalidacao { get; set; }
        public string LoginUsuarioLogado { get; set; } = null!;
        public Guid PerfilUsuarioLogado { get; set; }
        public bool? PossuiAnexo { get; set; }
    }
}
