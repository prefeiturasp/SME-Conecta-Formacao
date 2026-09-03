using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ImportacaoInscricao
{
    public class AlterarSituacaoImportacaoArquivoRegistroCommandHandlerTestes
    {
        private readonly Mock<IRepositorioImportacaoArquivoRegistro> _repositorioImportacaoArquivoRegistroMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoImportacaoArquivoRegistroCommandHandler _handler;

        public AlterarSituacaoImportacaoArquivoRegistroCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioImportacaoArquivoRegistroMock = mocker.GetMock<IRepositorioImportacaoArquivoRegistro>();
            _handler = mocker.CreateInstance<AlterarSituacaoImportacaoArquivoRegistroCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoImportacaoArquivoRegistroNula_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarSituacaoImportacaoArquivoRegistroCommand(
                _faker.Random.Long(1, 1000),
                SituacaoImportacaoArquivoRegistro.Erro,
                _faker.Random.String2(10));

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));
        }

        [Fact]
        public async Task DadoUmaImportacaoArquivoRegistroValida_QuandoExecutarHandle_DeveAtualizarERetornarTrue()
        {
            // Arrange
            var comando = new AlterarSituacaoImportacaoArquivoRegistroCommand(
                _faker.Random.Long(1, 1000),
                SituacaoImportacaoArquivoRegistro.Erro,
                _faker.Random.String2(10));

            var importacaoArquivoRegistro = new ImportacaoArquivoRegistro
            {
                Id = comando.Id,
                Situacao = SituacaoImportacaoArquivoRegistro.Validado
            };

            _repositorioImportacaoArquivoRegistroMock.Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(importacaoArquivoRegistro);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _repositorioImportacaoArquivoRegistroMock.Verify(r => r.Atualizar(It.Is<ImportacaoArquivoRegistro>(i =>
                i.Situacao == comando.Situacao &&
                i.Erro == comando.Erro)), Times.Once);
        }
    }
}
