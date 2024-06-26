using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ImportacaoArquivoRegistro : EntidadeBaseAuditavel
    {
        public long ImportacaoArquivoId { get; set; }
        public ImportacaoArquivo ImportacaoArquivo { get; set; }

        public int Linha { get; set; }
        public string Conteudo { get; set; }
        public string Erro { get; set; }

        public SituacaoImportacaoArquivoRegistro Situacao { get; set; }

        public void DefinirSituacao(SituacaoImportacaoArquivoRegistro situacao)
        {
            Situacao = situacao;
        }

        public void DefinirSituacaoErro(SituacaoImportacaoArquivoRegistro situacao, string erro)
        {
            Situacao = situacao;
            Erro = erro;
        }
    }
}