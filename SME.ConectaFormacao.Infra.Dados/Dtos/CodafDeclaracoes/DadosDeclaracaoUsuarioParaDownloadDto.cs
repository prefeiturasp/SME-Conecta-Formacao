namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes
{
    public class DadosDeclaracaoUsuarioParaDownloadDto
    {
        public long Id { get; set; }
        public long CodigoDeclaracao { get; set; }
        public string? ChaveObjetoArmazenamento { get; set; }
        public string NomeCompleto { get; set; } = null!;
        public string NomeFormacao { get; set; } = null!;
    }
}
