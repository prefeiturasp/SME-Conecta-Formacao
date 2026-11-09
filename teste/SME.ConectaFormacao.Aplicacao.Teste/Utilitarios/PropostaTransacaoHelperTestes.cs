using System.Data;
using FluentAssertions;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Log;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Utilitarios
{
    public class PropostaTransacaoHelperTestes
    {
        private readonly Mock<ITransacao> _transacaoMock = new();
        private readonly Mock<IDbTransaction> _dbTransactionMock = new();
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<IServicoLogs> _servicoLogsMock = new();

        public PropostaTransacaoHelperTestes()
        {
            _transacaoMock.Setup(t => t.Iniciar()).Returns(_dbTransactionMock.Object);
        }

        [Fact]
        public async Task DadoInserirComTransacao_QuandoSucesso_EntaoDeveComitarERetornarId()
        {
            // Arrange
            var proposta = new Proposta
            {
                NomeFormacao = "Formacao Teste",
                IntegrarNoSGA = true,
                AreaPromotoraId = 1,
                CriadoLogin = "usuario"
            };

            _repositorioPropostaMock
                .Setup(r => r.Inserir(proposta))
                .ReturnsAsync(999);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var id = await PropostaTransacaoHelper.InserirComTransacaoAsync(
                _transacaoMock.Object,
                _repositorioPropostaMock.Object,
                _mediatorMock.Object,
                _servicoLogsMock.Object,
                proposta,
                integrarNoSgaDto: true,
                operacao: "InserirProposta",
                CancellationToken.None);

            // Assert
            id.Should().Be(999);
            _repositorioPropostaMock.Verify(r => r.Inserir(proposta), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoInserirComTransacao_QuandoOcorrerErro_EntaoDeveFazerRollback()
        {
            // Arrange
            var proposta = new Proposta { NomeFormacao = "Formacao Erro" };

            _repositorioPropostaMock
                .Setup(r => r.Inserir(proposta))
                .ThrowsAsync(new InvalidOperationException("Erro de banco"));

            // Act
            var act = async () => await PropostaTransacaoHelper.InserirComTransacaoAsync(
                _transacaoMock.Object,
                _repositorioPropostaMock.Object,
                _mediatorMock.Object,
                _servicoLogsMock.Object,
                proposta,
                integrarNoSgaDto: null,
                operacao: "InserirProposta",
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoAtualizarComTransacao_QuandoSucesso_EntaoDeveAtualizarEComitar()
        {
            // Arrange
            var propostaBanco = new Proposta { Id = 10, IntegrarNoSGA = true, ArquivoImagemDivulgacaoId = 5 };
            var propostaDepois = new Proposta { Id = 10, IntegrarNoSGA = true, NomeFormacao = "Atualizada" };

            _repositorioPropostaMock
                .Setup(r => r.Atualizar(propostaDepois))
                .ReturnsAsync(propostaDepois);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await PropostaTransacaoHelper.AtualizarComTransacaoAsync(
                _transacaoMock.Object,
                _repositorioPropostaMock.Object,
                _mediatorMock.Object,
                _servicoLogsMock.Object,
                propostaBanco,
                propostaDepois,
                integrarNoSgaDto: null,
                operacao: "AlterarProposta",
                CancellationToken.None);

            // Assert
            _repositorioPropostaMock.Verify(r => r.Atualizar(propostaDepois), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task DadoAtualizarComTransacao_QuandoOcorrerErro_EntaoDeveFazerRollback()
        {
            // Arrange
            var propostaBanco = new Proposta { Id = 10 };
            var propostaDepois = new Proposta { Id = 10 };

            _repositorioPropostaMock
                .Setup(r => r.Atualizar(propostaDepois))
                .ThrowsAsync(new InvalidOperationException("Erro ao atualizar"));

            // Act
            var act = async () => await PropostaTransacaoHelper.AtualizarComTransacaoAsync(
                _transacaoMock.Object,
                _repositorioPropostaMock.Object,
                _mediatorMock.Object,
                _servicoLogsMock.Object,
                propostaBanco,
                propostaDepois,
                integrarNoSgaDto: null,
                operacao: "AlterarProposta",
                CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            _dbTransactionMock.Verify(t => t.Commit(), Times.Never);
            _dbTransactionMock.Verify(t => t.Dispose(), Times.Once);
        }
    }
}
