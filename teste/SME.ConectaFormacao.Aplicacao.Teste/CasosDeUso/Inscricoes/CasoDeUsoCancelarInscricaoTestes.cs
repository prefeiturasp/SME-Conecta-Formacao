using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoCancelarInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoCancelarInscricao _sut;

        public CasoDeUsoCancelarInscricaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoCancelarInscricao>();
        }

        [Fact]
        public async Task DadoIdValido_QuandoExecutar_EntaoDeveEnviarComandoERetornarResultado()
        {
            // Arrange
            var id = 123L;
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CancelarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(id);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<CancelarInscricaoCommand>(c => c.Id == id && c.Motivo == null), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
