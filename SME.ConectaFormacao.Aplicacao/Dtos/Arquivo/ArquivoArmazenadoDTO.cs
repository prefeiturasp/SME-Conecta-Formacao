namespace SME.ConectaFormacao.Aplicacao.Dtos.Arquivo
{
    public class ArquivoArmazenadoDTO
    {
        public ArquivoArmazenadoDTO(long id, Guid codigo, string path)
        {
            Id = id;
            Codigo = codigo;
            Path = path;
        }

        public long Id { get; set; }
        public Guid Codigo { get; set; }
        public string Path { get; set; }
    }
}
