using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class FiltroListagemTodasDeclaracoesCodafDto
    {
        public string? CodigoFormacao { get; set; }
        public string? NumeroHomologacao { get; set; }
        public string? NomeFormacao { get; set; }
        public string? CodigoDeclaracao { get; set; }
        public TipoDeclaracaoCodaf TipoDeclaracao { get; set; }
        public string? DocumentoCursista { get; set; }
        public string? DocumentoRegente { get; set; }
        public string? NomeCursista { get; set; }
        public DateTime? DataEmissao { get; set; }
        public long? EmissorId { get; set; }
        public long? TurmaId { get; set; }
        public required int Pagina { get; set; }
        public required int TamanhoPagina { get; set; }
    }
}
