using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class ArquivosImportadosTotalRegistro
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public SituacaoImportacaoArquivo Situacao { get; set; }
        public long TotalRegistros { get; set; }
        public long TotalProcessados { get; set; }
    }
}
