using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ConfirmarInscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class ConfirmarInscricaoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Faker _faker;
        private readonly ConfirmarInscricaoCommandHandler _handler;

        public ConfirmarInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _handler = mocker.CreateInstance<ConfirmarInscricaoCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoInscricaoNull_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new ConfirmarInscricaoCommand(_faker.Random.Long(1, 1000));
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoExcluida_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new ConfirmarInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = true
            };
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoComSituacaoDiferenteDeAguardandoAnalise_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new ConfirmarInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = false,
                Situacao = SituacaoInscricao.Confirmada
            };
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaInscricaoValida_QuandoExecutarHandle_DeveConfirmarInscricaoERetornarTrue()
        {
            // Arrange
            var comando = new ConfirmarInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = false,
                Situacao = SituacaoInscricao.AguardandoAnalise
            };
            var transacaoMock = new Mock<IDbTransaction>();
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            _repositorioInscricaoMock.Setup(r => r.ConfirmarInscricaoVaga(inscricao))
                .ReturnsAsync(true);
            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);
            // Assert
            resultado.Should().BeTrue();
            _repositorioInscricaoMock.Verify(r => r.Atualizar(It.Is<Inscricao>(i => i.Situacao == SituacaoInscricao.Confirmada)), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailConfirmacaoInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoUmaInscricaoValidaSemVagas_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new ConfirmarInscricaoCommand(_faker.Random.Long(1, 1000));
            var inscricao = new Inscricao
            {
                Excluido = false,
                Situacao = SituacaoInscricao.EmEspera
            };
            var transacaoMock = new Mock<IDbTransaction>();
            _repositorioInscricaoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);
            _repositorioInscricaoMock.Setup(r => r.ConfirmarInscricaoVaga(inscricao))
                .ReturnsAsync(false);
            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
            _repositorioInscricaoMock.Verify(r => r.Atualizar(It.IsAny<Inscricao>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailConfirmacaoInscricaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            transacaoMock.Verify(t => t.Commit(), Times.Never);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}
