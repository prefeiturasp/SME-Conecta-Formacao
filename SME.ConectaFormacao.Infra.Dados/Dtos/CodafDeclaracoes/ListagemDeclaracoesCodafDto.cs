using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class ListagemDeclaracoesCodafDto
    {
        public long Id { get; set; }
        public long NumeroHomologacao { get; set; }
        public long CodigoFormacao { get; set; }
        public long TurmaId { get; set; }
        public long EmissorId { get; set; }
        public string? NomeFormacao { get; set; }
        public string? NomeEmissor { get; set; } = null;
        public string DocumentoCursista { get; set; } = null!;
        public string? DocumentoRegente { get; set; } = null!;
        public long CodigoDeclaracao { get; set; }
        public TipoDeclaracaoCodaf TipoDeclaracao { get; set; }
        public TipoEmissor? TipoEmissor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string? NomeCursista { get; set; }
        public string? NomeRegente { get; set; }
    }
}