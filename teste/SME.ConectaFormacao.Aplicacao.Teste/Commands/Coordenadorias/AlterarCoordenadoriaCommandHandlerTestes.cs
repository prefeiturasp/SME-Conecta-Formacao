using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Coordenadorias
{
    public class AlterarCoordenadoriaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioCoordenadoria> _repositorioCoordenadoria;
        private readonly AlterarCoordenadoriaCommandHandler _sut;
        private readonly Faker _faker;

        public AlterarCoordenadoriaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCoordenadoria = mocker.GetMock<IRepositorioCoordenadoria>();
            _sut = mocker.CreateInstance<AlterarCoordenadoriaCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoInformacoesValidas_QuandoAlterar_DeveSalvarCoordenadoria()
        {
            // Arrange
            var command = new AlterarCoordenadoriaCommand(1, _faker.Company.CompanyName(), _faker.Random.AlphaNumeric(5).ToUpper());
            _repositorioCoordenadoria
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(It.IsAny<long>()))
                .ReturnsAsync(new Coordenadoria() { Id = command.Id, Nome = "Coordenadoria Antiga", Sigla = "CA" });

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            _repositorioCoordenadoria
                .Verify(r => r.Atualizar(It.Is<Coordenadoria>(c => 
                                                                c.Id == command.Id &&
                                                                c.Nome == command.Nome && 
                                                                c.Sigla == command.Sigla))
                , Times.Once);
            result.Sucesso.Should().BeTrue();
        }

        [Fact]
        public async Task DadoInformacoesInvalidas_QuandoAlterar_DeveRetornarErro()
        {
            // Arrange
            var command = new AlterarCoordenadoriaCommand(1, _faker.Company.CompanyName(), _faker.Random.AlphaNumeric(5).ToUpper());

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            _repositorioCoordenadoria.Verify(r => r.Atualizar(It.IsAny<Coordenadoria>()), Times.Never);
            result.Sucesso.Should().BeFalse();
            result.MensagensErro.Should().Contain("Coordenadoria não encontrada.");
        }
    }
}
