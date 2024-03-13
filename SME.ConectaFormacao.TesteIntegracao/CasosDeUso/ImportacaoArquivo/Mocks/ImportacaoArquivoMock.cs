using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks
{
    public class ImportacaoArquivoMock
    {
        public static ImportacaoArquivoInscricaoDTO GerarImportacaoArquivoVazia()
        {
            var importacaoArquivoInscricaoDTO = new ImportacaoArquivoInscricaoDTO()
            {
                Arquivo = new Mock<IFormFile>().Object,
            };

            return importacaoArquivoInscricaoDTO;
        }
        
        public static ImportacaoArquivoInscricaoDTO GerarImportacaoArquivoValida()
        {
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("importacao_arquivo").Verifiable();
            file.Setup(f => f.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            
            var bytes = Encoding.UTF8.GetBytes("conteudo do arquivo");
            var ms = new MemoryStream(bytes);

            file.Setup(f => f.Length).Returns(ms.Length);
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken cancellationToken) => ms.CopyToAsync(stream, cancellationToken))
                .Verifiable();
            
            var importacaoArquivoInscricaoDTO = new ImportacaoArquivoInscricaoDTO()
            {
                Arquivo = file.Object,
                Nome = "importacao_arquivo.xlsx",
                PropostaId = 1,
                Situacao = SituacaoImportacaoArquivo.Enviado,
                Tipo = TipoImportacaoArquivo.Inscricao_Manual
            };

            return importacaoArquivoInscricaoDTO;
        }
    }
}
