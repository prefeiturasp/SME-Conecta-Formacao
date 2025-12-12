using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo
{
    public class ImportacaoArquivoRegistroDto
    {
        public long Id { get; set; }
        public long ImportacaoArquivoId { get; set; }

        public int Linha { get; set; }
        public string Conteudo { get; set; } = null!;
        public string? Erro { get; set; }

        public SituacaoImportacaoArquivoRegistro Situacao { get; set; }
        public long PropostaId { get; set; }
    }
}
