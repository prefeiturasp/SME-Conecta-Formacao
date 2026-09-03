using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ImportacaoInscricao
{
    public class AlterarSituacaoImportacaoArquivoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioImportacaoArquivo> _repositorioImportacaoArquivoMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoImportacaoArquivoCommandHandler _handler;

        public AlterarSituacaoImportacaoArquivoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioImportacaoArquivoMock = mocker.GetMock<IRepositorioImportacaoArquivo>();
            _handler = mocker.CreateInstance<AlterarSituacaoImportacaoArquivoCommandHandler>();
            _faker = new();
        }

        [Fact]
        public async Task DadoImportacaoArquivoNull_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarSituacaoImportacaoArquivoCommand(_faker.Random.Long(1, 1000), _faker.Random.Enum<SituacaoImportacaoArquivo>());

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaImportacaoArquivoValida_QuandoExecutarHandle_DeveAtualizarERetornarTrue()
        {
            // Arrange
            var comando = new AlterarSituacaoImportacaoArquivoCommand(_faker.Random.Long(1, 1000), _faker.Random.Enum<SituacaoImportacaoArquivo>());
            var importacaoArquivo = new Dominio.Entidades.ImportacaoArquivo();

            _repositorioImportacaoArquivoMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(importacaoArquivo);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioImportacaoArquivoMock.Verify(r => r.Atualizar(It.IsAny<Dominio.Entidades.ImportacaoArquivo>()), Times.Once);
        }
    }
}
