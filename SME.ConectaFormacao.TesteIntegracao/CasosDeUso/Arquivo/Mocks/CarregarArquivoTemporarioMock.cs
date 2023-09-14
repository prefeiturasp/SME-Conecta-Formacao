﻿using Microsoft.AspNetCore.Http;
using Moq;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks
{
    public class CarregarArquivoTemporarioMock
    {
        private const int DEZ_MB = 10 * 1024 * 1024;

        private const string TIPO_CONTEUDO = "image/jpeg";
        private const string NOME_ARQUIVO = "imagem.jpg";
        private const string IMAGEM_BASE64 = "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";

        public static IFormFile GerarFormFileVazio()
        {
            var formFileMock = new Mock<IFormFile>();
            return formFileMock.Object;
        }

        public static IFormFile GerarFormFileMaiorQueDezMb()
        {
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(DEZ_MB + 1);

            return file.Object;
        }

        public static IFormFile GerarFormFileValido()
        {
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns(NOME_ARQUIVO).Verifiable();
            file.Setup(f => f.ContentType).Returns(TIPO_CONTEUDO);

            var bytes = Convert.FromBase64String(IMAGEM_BASE64);
            var ms = new MemoryStream(bytes);

            file.Setup(f => f.Length).Returns(ms.Length);
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns((Stream stream, CancellationToken cancellationToken) => ms.CopyToAsync(stream, cancellationToken))
                    .Verifiable();

            return file.Object;
        }
    }
}
