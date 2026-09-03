using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Propostas
{
    public class AlterarPropostaRascunhoCommandHandlerTestes
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly AlterarPropostaRascunhoCommandHandler _handler;

        public AlterarPropostaRascunhoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _handler = mocker.CreateInstance<AlterarPropostaRascunhoCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutarHandle_EntaoDeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarPropostaRascunhoCommand(_faker.Random.Long(1, 1000), new PropostaDTO());

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoErroAoSalvar_QuandoExecutarHandle_EntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var comando = new AlterarPropostaRascunhoCommand(_faker.Random.Long(1, 1000), new PropostaDTO());
            var proposta = new Proposta { Id = comando.Id };
            var propostaMapeada = new Proposta { Id = comando.Id };
            var transacaoMock = new Mock<IDbTransaction>();

            _repositorioPropostaMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(proposta);
            _mapperMock.Setup(m => m.Map<Proposta>(comando.PropostaDTO))
                .Returns(propostaMapeada);
            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            
            _repositorioPropostaMock.Setup(r => r.Atualizar(It.IsAny<Proposta>()))
                .ThrowsAsync(new Exception("Erro no banco"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _handler.Handle(comando, CancellationToken.None));

            transacaoMock.Verify(t => t.Rollback(), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaValida_QuandoExecutarHandle_EntaoDeveAlterarComSucesso()
        {
            // Arrange
            var comando = new AlterarPropostaRascunhoCommand(_faker.Random.Long(1, 1000), new PropostaDTO());
            var proposta = new Proposta { Id = comando.Id, AreaPromotoraId = _faker.Random.Long(1, 100), AcaoFormativaTexto = "Texto", AcaoFormativaLink = "Link" };
            var propostaMapeada = new Proposta();
            var transacaoMock = new Mock<IDbTransaction>();

            _repositorioPropostaMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(proposta);
            _mapperMock.Setup(m => m.Map<Proposta>(comando.PropostaDTO))
                .Returns(propostaMapeada);
            _transacaoMock.Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.EntidadeId.Should().Be(comando.Id);
            
            _repositorioPropostaMock.Verify(r => r.Atualizar(propostaMapeada), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }
    }
}
