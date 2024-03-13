using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class ImportacaoArquivoDTO
    {
        public ImportacaoArquivoDTO(long propostaId, string nome, TipoImportacaoArquivo tipo, SituacaoImportacaoArquivo situacao)
        {
            PropostaId = propostaId;
            Nome = nome;
            Tipo = tipo;
            Situacao = situacao;
        }
        
        public long Id { get; set; }
        public long PropostaId { get; set; }
        
        public string Nome { get; set; }
        
        public TipoImportacaoArquivo Tipo { get; set; }
        public SituacaoImportacaoArquivo Situacao { get; set; }
    }
}
