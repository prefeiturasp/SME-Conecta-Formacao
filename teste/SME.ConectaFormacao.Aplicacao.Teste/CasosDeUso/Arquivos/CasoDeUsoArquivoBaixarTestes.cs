using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Arquivos
{
    public class CasoDeUsoArquivoBaixarTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoArquivoBaixar _casoDeUso;

        public CasoDeUsoArquivoBaixarTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoArquivoBaixar>();
        }

        [Fact]
        public async Task DadoCodigoArquivo_QuandoExecutar_EntaoDeveRetornarArquivoBaixadoDTO()
        {
            // Arrange
            var codigoArquivo = Guid.NewGuid();
            var arquivoEsperado = new ArquivoBaixadoDTO();

            _mediatorMock.Setup(m => m.Send(It.Is<ObterArquivoBaixarQuery>(q => q.Codigo == codigoArquivo), default))
                .ReturnsAsync(arquivoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(codigoArquivo);

            // Assert
            resultado.Should().Be(arquivoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterArquivoBaixarQuery>(q => q.Codigo == codigoArquivo), default), Times.Once);
        }
    }
}
