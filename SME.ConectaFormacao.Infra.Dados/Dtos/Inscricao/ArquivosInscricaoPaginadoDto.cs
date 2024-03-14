namespace SME.ConectaFormacao.Infra.Dados.Dtos.Inscricao
{
    public class ArquivosInscricaoPaginadoDto
    {
        public IEnumerable<ArquivosImportadosDto> Arquivos { get; set; }
        public int TotalDeRegistros { get; set; }
    }
}
