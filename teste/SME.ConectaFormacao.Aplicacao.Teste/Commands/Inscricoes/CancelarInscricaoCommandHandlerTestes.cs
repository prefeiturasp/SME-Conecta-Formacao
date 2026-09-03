using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.CancelarInscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class CancelarInscricaoCommandHandlerTestes
    {
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IDbTransaction> _dbTransactionMock;
        private readonly CancelarInscricaoCommandHandler _sut;

        public CancelarInscricaoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _dbTransactionMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);

            _sut = mocker.CreateInstance<CancelarInscricaoCommandHandler>();
        }

        [Fact]
        public async Task DadoInscricaoNula_QuandoChamarHandle_EntaoRetornaVerdadeiroNaoFazNada()
        {
            // Arrange
            var comando = new CancelarInscricaoCommand(1, "Motivo teste");

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _transacaoMock.Verify(t => t.Iniciar(), Times.Never);
        }

        [Fact]
        public async Task DadoInscricaoValidaConfirmada_QuandoChamarHandle_EntaoDeveLiberarVagaECancelar()
        {
            // Arrange
            var comando = new CancelarInscricaoCommand(2, "Desistencia");
            var inscricao = new Inscricao { Id = 2, Situacao = SituacaoInscricao.Confirmada };

            _repositorioInscricaoMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(2))
                .ReturnsAsync(inscricao);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            inscricao.SituacaoAnterior.Should().Be(SituacaoInscricao.Confirmada);
            inscricao.Situacao.Should().Be(SituacaoInscricao.Cancelada);
            inscricao.MotivoCancelamento.Should().Be("Desistencia");

            _repositorioInscricaoMock.Verify(r => r.LiberarInscricaoVaga(inscricao), Times.Once);
            _repositorioInscricaoMock.Verify(r => r.Atualizar(inscricao), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarEmailCancelarInscricaoCommand>(c => c.InscricaoId == 2 && c.Motivo == "Desistencia"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoInscricaoValidaEmEspera_QuandoChamarHandle_EntaoNaoLiberaVagaApenasCancela()
        {
            // Arrange
            var comando = new CancelarInscricaoCommand(3, "Outro motivo");
            var inscricao = new Inscricao { Id = 3, Situacao = SituacaoInscricao.EmEspera };

            _repositorioInscricaoMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(3))
                .ReturnsAsync(inscricao);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            inscricao.SituacaoAnterior.Should().Be(SituacaoInscricao.EmEspera);
            inscricao.Situacao.Should().Be(SituacaoInscricao.Cancelada);
            inscricao.MotivoCancelamento.Should().Be("Outro motivo");

            _repositorioInscricaoMock.Verify(r => r.LiberarInscricaoVaga(It.IsAny<Inscricao>()), Times.Never);
            _repositorioInscricaoMock.Verify(r => r.Atualizar(inscricao), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}
