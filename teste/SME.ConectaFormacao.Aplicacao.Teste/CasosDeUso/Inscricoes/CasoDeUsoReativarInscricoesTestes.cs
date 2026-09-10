using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ReativarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoReativarInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoReativarInscricoes _sut;

        public CasoDeUsoReativarInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoReativarInscricoes>();
        }

        [Fact]
        public async Task DadoIdsNulo_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            long[]? ids = null;

            // Act
            Func<Task> act = async () => await _sut.Executar(ids!);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.INSCRICOES_REATIVADAS_COM_PROBLEMA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoIdsVazio_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var ids = Array.Empty<long>();

            // Act
            Func<Task> act = async () => await _sut.Executar(ids);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.INSCRICOES_REATIVADAS_COM_PROBLEMA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmUnicoIdComFalha_QuandoExecutar_EntaoDeveRelancarNegocioExceptionComMensagens()
        {
            // Arrange
            var ids = new long[] { 10L };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Erro ao reativar inscrição."));

            // Act
            Func<Task> act = async () => await _sut.Executar(ids);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage("*Erro ao reativar inscrição.*");
            _mediatorMock.Verify(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 10L), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoMultiplosIdsComFalhaEmAlguns_QuandoExecutar_EntaoDeveRetornarSucessoComInconsistencias()
        {
            // Arrange
            var ids = new long[] { 20L, 10L };
            _mediatorMock
                .Setup(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 10L), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NegocioException("Inscrição 10 não pode ser reativada."));
            _mediatorMock
                .Setup(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 20L), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_REATIVADAS_COM_INCONSISTENCIAS);
            _mediatorMock.Verify(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 10L), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 20L), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoMultiplosIdsComSucessoEmTodos_QuandoExecutar_EntaoDeveEnviarCommandParaCadaERetornarSucesso()
        {
            // Arrange
            var ids = new long[] { 20L, 10L };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(ids);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be(MensagemNegocio.INSCRICOES_REATIVACAO_CONFIRMADAS_COM_SUCESSO);
            _mediatorMock.Verify(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 10L), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ReativarInscricaoCommand>(c => c.Id == 20L), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
