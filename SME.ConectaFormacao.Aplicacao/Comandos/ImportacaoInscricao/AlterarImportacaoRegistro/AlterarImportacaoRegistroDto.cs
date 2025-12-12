using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro
{
    public class AlterarImportacaoRegistroDto
    {
        public long Id { get; set; }
        public SituacaoImportacaoArquivoRegistro Situacao { get; set; }
        public string Conteudo { get; set; } = null!;
        public string? Erro { get; set; }
    }
}
