using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo
{
    public class ImportacaoArquivoInscricaoDTO
    {
        public long Id { get; set; }
        public long PropostaId { get; set; }
        
        public string Nome { get; set; }

        public TipoImportacaoArquivo Tipo { get; set; }
        public SituacaoImportacaoArquivo Situacao { get; set; }
    }
}
