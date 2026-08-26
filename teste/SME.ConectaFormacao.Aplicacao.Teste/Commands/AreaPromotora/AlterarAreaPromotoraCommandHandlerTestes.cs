using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.AreaPromotora
{
    public class AlterarAreaPromotoraCommandHandlerTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioAreaPromotora> _repositorioAreaPromotoraMock;
        private readonly Mock<ICacheDistribuido> _cacheDistribuidoMock;
        private readonly Faker _faker;
        private readonly AlterarAreaPromotoraCommandHandler _handler;

        public AlterarAreaPromotoraCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioAreaPromotoraMock = mocker.GetMock<IRepositorioAreaPromotora>();
            _cacheDistribuidoMock = mocker.GetMock<ICacheDistribuido>();
            _handler = mocker.CreateInstance<AlterarAreaPromotoraCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoAreaPromotoraInexistente_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new AreaPromotoraDTO { DreId = null, GrupoId = Guid.NewGuid(), Tipo = AreaPromotoraTipo.RedeDireta };
            var comando = new AlterarAreaPromotoraCommand(id, dto);

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoAreaPromotoraValidaComDreId_QuandoExecutarHandle_DeveValidarDreEAtualizarAreaPromotora()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new AreaPromotoraDTO
            {
                DreId = _faker.Random.Long(1, 1000),
                GrupoId = Guid.NewGuid(),
                Tipo = AreaPromotoraTipo.RedeDireta,
                Telefones = []
            };
            var comando = new AlterarAreaPromotoraCommand(id, dto);
            var areaPromotora = new Dominio.Entidades.AreaPromotora { Id = id };
            var dbTransactionMock = new Mock<IDbTransaction>();

            _repositorioAreaPromotoraMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(areaPromotora);

            _mapperMock.Setup(m => m.Map<Dominio.Entidades.AreaPromotora>(dto))
                .Returns(new Dominio.Entidades.AreaPromotora { Id = id });

            _repositorioAreaPromotoraMock.Setup(r => r.ObterTelefonesPorId(comando.Id))
                .ReturnsAsync([]);

            _mapperMock.Setup(m => m.Map<IEnumerable<AreaPromotoraTelefone>>(dto.Telefones))
                .Returns([]);

            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(dbTransactionMock.Object);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ValidarPerfilDreAreaPromotoraCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ValidarEmailsAreaPromotoraCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _repositorioAreaPromotoraMock.Verify(r => r.Atualizar(dbTransactionMock.Object, It.IsAny<Dominio.Entidades.AreaPromotora>()), Times.Once);
            dbTransactionMock.Verify(t => t.Commit(), Times.Once);
            _cacheDistribuidoMock.Verify(c => c.RemoverAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DadoAreaPromotoraValidaComTelefones_QuandoExecutarHandle_DeveInserirERemoverTelefones()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new AreaPromotoraDTO
            {
                DreId = null,
                GrupoId = Guid.NewGuid(),
                Tipo = AreaPromotoraTipo.RedeDireta,
                Telefones = []
            };
            var comando = new AlterarAreaPromotoraCommand(id, dto);
            var areaPromotora = new Dominio.Entidades.AreaPromotora { Id = id };
            var dbTransactionMock = new Mock<IDbTransaction>();
            var telefoneAntigo = new AreaPromotoraTelefone { Telefone = "11111111" };
            var telefoneNovo = new AreaPromotoraTelefone { Telefone = "22222222" };

            _repositorioAreaPromotoraMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(areaPromotora);

            _mapperMock.Setup(m => m.Map<Dominio.Entidades.AreaPromotora>(dto))
                .Returns(new Dominio.Entidades.AreaPromotora { Id = id });

            _repositorioAreaPromotoraMock.Setup(r => r.ObterTelefonesPorId(comando.Id))
                .ReturnsAsync([telefoneAntigo]);

            _mapperMock.Setup(m => m.Map<IEnumerable<AreaPromotoraTelefone>>(dto.Telefones))
                .Returns([telefoneNovo]);

            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(dbTransactionMock.Object);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ValidarGrupoAreaPromotoraCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _repositorioAreaPromotoraMock.Verify(r => r.InserirTelefones(dbTransactionMock.Object, id, It.Is<IEnumerable<AreaPromotoraTelefone>>(t => t.Any(x => x.Telefone == "22222222"))), Times.Once);
            _repositorioAreaPromotoraMock.Verify(r => r.RemoverTelefones(dbTransactionMock.Object, id, It.Is<IEnumerable<AreaPromotoraTelefone>>(t => t.Any(x => x.Telefone == "11111111"))), Times.Once);
            dbTransactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoBanco_QuandoExecutarHandle_DeveFazerRollbackDalancarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new AreaPromotoraDTO { DreId = null, GrupoId = Guid.NewGuid(), Tipo = AreaPromotoraTipo.RedeDireta };
            var comando = new AlterarAreaPromotoraCommand(id, dto);
            var areaPromotora = new Dominio.Entidades.AreaPromotora { Id = id };
            var dbTransactionMock = new Mock<IDbTransaction>();

            _repositorioAreaPromotoraMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(areaPromotora);

            _mapperMock.Setup(m => m.Map<Dominio.Entidades.AreaPromotora>(dto))
                .Returns(new Dominio.Entidades.AreaPromotora { Id = id });

            _repositorioAreaPromotoraMock.Setup(r => r.ObterTelefonesPorId(comando.Id))
                .ReturnsAsync([]);

            _mapperMock.Setup(m => m.Map<IEnumerable<AreaPromotoraTelefone>>(dto.Telefones))
                .Returns([]);

            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(dbTransactionMock.Object);

            _repositorioAreaPromotoraMock.Setup(r => r.Atualizar(It.IsAny<IDbTransaction>(), It.IsAny<Dominio.Entidades.AreaPromotora>()))
                .ThrowsAsync(new Exception("Erro de banco"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(comando, CancellationToken.None));
            dbTransactionMock.Verify(t => t.Rollback(), Times.Once);
            dbTransactionMock.Verify(t => t.Commit(), Times.Never);
        }
    }
}
