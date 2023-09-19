using Microsoft.AspNetCore.Http;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Arquivo
{
    public class ArquivoDTO
    {
        public ArquivoDTO(string nome, Guid codigo, TipoArquivo tipo, string tipoConteudo, IFormFile formFile)
        {
            Nome = nome;
            Codigo = codigo;
            Tipo = tipo;
            TipoConteudo = tipoConteudo;
            FormFile = formFile;
        }

        public string Nome { get; set; }
        public Guid Codigo { get; set; }
        public TipoArquivo Tipo { get; set; }
        public string TipoConteudo { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
