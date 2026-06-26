using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Log;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Aplicacao.Dtos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoSalvarLogTestes
    {
        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoSalvarLog casoDeUso;

        public CasoDeUsoSalvarLogTestes()
        {
            mediatorMock = new Mock<IMediator>();
            casoDeUso = new CasoDeUsoSalvarLog(mediatorMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_mediator_retornar_true()
        {
            var dto = new LogDTO();

            mediatorMock
                .Setup(x => x.Send(
                    It.Is<SalvarLogCommand>(c => c.LogDTO == dto),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await casoDeUso.Executar(dto);

            Assert.True(resultado);

            mediatorMock.Verify(x => x.Send(
                It.Is<SalvarLogCommand>(c => c.LogDTO == dto),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_mediator_retornar_false()
        {
            var dto = new LogDTO();

            mediatorMock
                .Setup(x => x.Send(
                    It.Is<SalvarLogCommand>(c => c.LogDTO == dto),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await casoDeUso.Executar(dto);

            Assert.False(resultado);

            mediatorMock.Verify(x => x.Send(
                It.Is<SalvarLogCommand>(c => c.LogDTO == dto),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
