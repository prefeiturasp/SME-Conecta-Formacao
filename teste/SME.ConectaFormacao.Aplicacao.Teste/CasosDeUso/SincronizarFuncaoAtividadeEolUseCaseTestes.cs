using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLogViaRabbit;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class SincronizarFuncaoAtividadeEolUseCaseTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly SincronizarFuncaoAtividadeEolUseCase _useCase;

        public SincronizarFuncaoAtividadeEolUseCaseTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _useCase = mocker.CreateInstance<SincronizarFuncaoAtividadeEolUseCase>();
        }

        [Fact]
        public async Task DadoQueConsultaDresRetornaDados_QuandoExecutar_EntaoDevePublicarMensagensParaCadaDreEParaSme()
        {

            var dresEol = new List<DreServicoEol>
            {
                new (){Codigo = "DRE1"},
                new (){Codigo = "DRE2"}
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dresEol);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);


            var resultado = await _useCase.Executar(new());


            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Filtros.ToString() == "SME"), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Filtros.ToString() == "DRE1"), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Filtros.ToString() == "DRE2"), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task DadoQueConsultaDresNaoRetornaDados_QuandoExecutar_EntaoDevePublicarMensagensSomenteParaSme()
        {

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);


            var resultado = await _useCase.Executar(new());


            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(cmd => cmd.Filtros.ToString() == "SME"), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoQuePublicacaoRetornaFalse_QuandoExecutar_EntaoDeveSalvarLogNegocio()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            await _useCase.Executar(new());

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarLogViaRabbitCommand>(log =>
                    log.Nivel == LogNivel.Negocio &&
                    log.Contexto == LogContexto.SincronizacaoCargosEol &&
                    log.Mensagem.Contains("Erro ao publicar mensagem na fila para sincronização de função atividade EOL")), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoQueOcorreExcecaoNoLoop_QuandoExecutar_EntaoDeveSalvarLogCriticoEContinuar()
        {

            _mediatorMock
                .SetupSequence(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro crítico na publicação"));


            await _useCase.Executar(new());

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterCodigosDresEOLQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarLogViaRabbitCommand>(log =>
                    log.Nivel == LogNivel.Critico &&
                    log.Contexto == LogContexto.SincronizacaoCargosEol &&
                    log.Mensagem.Contains("Erro ao sincronizar função atividade EOL da DRE")), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}
