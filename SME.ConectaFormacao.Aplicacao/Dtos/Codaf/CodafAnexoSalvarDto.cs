using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Codaf
{
    public class CodafAnexoSalvarDto
    {
        public Guid ArquivoCodigo { get; set; }
        public required string NomeArquivo { get; set; }
        public TipoAnexoCodaf TipoAnexoId { get; set; }
    }
}