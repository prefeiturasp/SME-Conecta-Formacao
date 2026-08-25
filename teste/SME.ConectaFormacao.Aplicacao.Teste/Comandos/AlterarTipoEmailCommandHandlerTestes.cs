using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos;
using SME.ConectaFormacao.Aplicacao.Comandos.Usuarios.AlterarTipoEmail;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos
{
    public class AlterarTipoEmailCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly AlterarTipoEmailCommandHandler _sut;

        public AlterarTipoEmailCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<AlterarTipoEmailCommandHandler>();
        }

        [Fact]
        public async Task DadoAtualizacaoBemSucedida_QuandoExecutar_EntaoRemoveCacheERetornaTrue()
        {
            // Arrange
            var comando = new AlterarTipoEmailCommand(1, "");

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(m => m.AtualizarTipoEmail(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DadoFalhaNaAtualizacao_QuandoExecutar_EntaoLancaExcecaoENaoRemoveCache()
        {
            // Arrange
            var comando = new AlterarTipoEmailCommand(1, "");

            _mocker.GetMock<IRepositorioUsuario>()
                .Setup(m => m.AtualizarTipoEmail(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            Func<Task> acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<NegocioException>();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
