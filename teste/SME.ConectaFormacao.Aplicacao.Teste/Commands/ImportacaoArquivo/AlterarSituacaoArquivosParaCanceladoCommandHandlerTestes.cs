using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ImportacaoArquivo
{
    public class AlterarSituacaoArquivosParaCanceladoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioImportacaoArquivo> _repositorioImportacaoArquivoMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoArquivosParaCanceladoCommandHandler _handler;

        public AlterarSituacaoArquivosParaCanceladoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioImportacaoArquivoMock = mocker.GetMock<IRepositorioImportacaoArquivo>();
            _handler = mocker.CreateInstance<AlterarSituacaoArquivosParaCanceladoCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoArquivoNulo_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarSituacaoArquivosParaCanceladoCommand(_faker.Random.Long(1, 1000));
            _repositorioImportacaoArquivoMock.Setup(r => r.ObterPorId(comando.ArquivoImportacaoId))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.ImportacaoArquivo)null);

            // Act
            var act = () => _handler.Handle(comando, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NegocioException>();
        }

        [Fact]
        public async Task DadoArquivoValido_QuandoExecutarHandle_DeveAlterarSituacaoParaCanceladoEAtualizar()
        {
            // Arrange
            var comando = new AlterarSituacaoArquivosParaCanceladoCommand(_faker.Random.Long(1, 1000));
            var arquivo = new SME.ConectaFormacao.Dominio.Entidades.ImportacaoArquivo();
            
            _repositorioImportacaoArquivoMock.Setup(r => r.ObterPorId(comando.ArquivoImportacaoId))
                .ReturnsAsync(arquivo);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioImportacaoArquivoMock.Verify(r => r.Atualizar(It.Is<SME.ConectaFormacao.Dominio.Entidades.ImportacaoArquivo>(a => a.Situacao == SituacaoImportacaoArquivo.Cancelado)), Times.Once);
        }
    }
}
