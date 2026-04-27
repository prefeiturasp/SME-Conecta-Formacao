using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.InserirCoordenadoria;
using SME.ConectaFormacao.Aplicacao.Dtos.Coordenadorias;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Coordenadorias
{
    public class InserirCoordenadoriaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioCoordenadoria> _repositorioCoordenadoria;
        private readonly Mock<IMapper> _mapper;
        private readonly InserirCoordenadoriaCommandHandler _sut;
        private readonly Faker _faker;

        public InserirCoordenadoriaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCoordenadoria = mocker.GetMock<IRepositorioCoordenadoria>();
            _mapper = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<InserirCoordenadoriaCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoInformacoesValidas_QuandoInserir_DeveSalvarCoordenadoria()
        {
            // Arrange
            var command = new InserirCoordenadoriaCommand(_faker.Company.CompanyName(), _faker.Random.AlphaNumeric(5).ToUpper());
            _mapper.Setup(m => m.Map<CoordenadoriaDto>(It.IsAny<Coordenadoria>()))
                   .Returns(new CoordenadoriaDto() { Nome = command.Nome, Sigla = command.Sigla, Id = 1});

            _repositorioCoordenadoria
                .Setup(r => r.Inserir(It.IsAny<Coordenadoria>()))
                .ReturnsAsync(1);

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            _repositorioCoordenadoria.Verify(r => r.Inserir(It.Is<Coordenadoria>(c => c.Nome == command.Nome && c.Sigla == command.Sigla)), Times.Once);
            result.Sucesso.Should().BeTrue();
            result.Dados.Should().NotBeNull();
            result.Dados!.Nome.Should().Be(command.Nome);
            result.Dados.Sigla.Should().Be(command.Sigla);
            result.Dados.Id.Should().BeGreaterThan(0);
        }
    }
}
