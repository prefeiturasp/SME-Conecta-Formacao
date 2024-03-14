using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks
{
    public class ImportacaoArquivoMock
    {
        public static IFormFile GerarArquivoValido()
        {
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("importacao_arquivo.xlsx").Verifiable();
            file.Setup(f => f.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            
            var bytes = Encoding.UTF8.GetBytes("Turma; Colaborador da rede; Registro funcional;");
            var ms = new MemoryStream(bytes);

            file.Setup(f => f.Length).Returns(ms.Length);
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken cancellationToken) => ms.CopyToAsync(stream, cancellationToken))
                .Verifiable();
            
            return file.Object;
        }
    }
}
