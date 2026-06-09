using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class ExecutarSincronizacaoInstitucionalDreSyncUseCaseTestes
    {
        private readonly AutoMocker mocker;
        private readonly Mock<MediatR.IMediator> mediatorMock;
        private readonly ExecutarSincronizacaoInstitucionalDreSyncUseCase useCase;

        public ExecutarSincronizacaoInstitucionalDreSyncUseCaseTestes()
        {
            mocker = new AutoMocker();
            mediatorMock = mocker.GetMock<MediatR.IMediator>();
            useCase = mocker.CreateInstance<ExecutarSincronizacaoInstitucionalDreSyncUseCase>();
        }

        [Fact]
        public async Task DadoNenhumaDre_RetornaNegocioException_QuandoNull()
        {
            // Arrange
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<DreServicoEol>?)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NegocioException>(() => useCase.Executar(new Infra.Servicos.Rabbit.Dto.MensagemRabbit()));
            Assert.Equal(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL, ex.Message);
        }

        [Fact]
        public async Task DadoNenhumaDre_RetornaNegocioException_QuandoListaVazia()
        {
            // Arrange
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<DreServicoEol>());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NegocioException>(() => useCase.Executar(new Infra.Servicos.Rabbit.Dto.MensagemRabbit()));
            Assert.Equal(MensagemNegocio.NENHUMA_DRE_ENCONTRADA_NO_EOL, ex.Message);
        }

        [Fact]
        public async Task DadoDre_QuandoPublicarRetornaTrue_DeveRetornarTrueESemSalvarLog()
        {
            // Arrange
            var dre = new DreServicoEol("01", "DRE Teste", "DRE");
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { dre });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await useCase.Executar(new Infra.Servicos.Rabbit.Dto.MensagemRabbit());

            // Assert
            Assert.True(resultado);
            mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SalvarLogViaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoDre_QuandoPublicarRetornaFalse_DeveSalvarLogERetornarTrue()
        {
            // Arrange
            var dre = new DreServicoEol("02", "DRE Log", "DRE");
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { dre });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarLogViaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await useCase.Executar(new Infra.Servicos.Rabbit.Dto.MensagemRabbit());

            // Assert
            Assert.True(resultado);
            mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediatorMock.Verify(m => m.Send(It.IsAny<SalvarLogViaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoDre_QuandoPublicarLancaException_DeveLancarNegocioExceptionComNomeDre()
        {
            // Arrange
            var dre = new DreServicoEol("03", "DRE Excecao", "DRE");
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { dre });

            mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("erro interno"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NegocioException>(() => useCase.Executar(new Infra.Servicos.Rabbit.Dto.MensagemRabbit()));
            Assert.Contains(dre.Nome, ex.Message);
        }
    }
}
