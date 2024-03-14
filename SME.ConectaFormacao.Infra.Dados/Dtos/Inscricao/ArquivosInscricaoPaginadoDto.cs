namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricao
{
    public class ArquivosInscricaoPaginadoDTO
    {
        public IEnumerable<ArquivosImportadosDTO> Arquivos { get; set; }
        public int TotalDeRegistros { get; set; }
    }
}
