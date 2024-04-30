using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ImportacaoArquivo : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public Proposta Proposta { get; set; }

        public string Nome { get; set; }

        public TipoImportacaoArquivo Tipo { get; set; }
        public SituacaoImportacaoArquivo Situacao { get; set; }

        public void DefinirSituacao(SituacaoImportacaoArquivo situacao)
        {
            Situacao = situacao;
        }
    }
}