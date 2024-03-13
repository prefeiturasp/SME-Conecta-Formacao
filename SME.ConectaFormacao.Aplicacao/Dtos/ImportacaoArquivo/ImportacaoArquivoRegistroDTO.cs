using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class ImportacaoArquivoRegistroDTO
    {
        public long Id { get; set; }
        public long ImportacaoArquivoId { get; set; }
        
        public int Linha { get; set; }
        public string Conteudo { get; set; }
        public string Erro { get; set; }
        
        public SituacaoImportacaoArquivoRegistro Situacao { get; set; }
    }
}
