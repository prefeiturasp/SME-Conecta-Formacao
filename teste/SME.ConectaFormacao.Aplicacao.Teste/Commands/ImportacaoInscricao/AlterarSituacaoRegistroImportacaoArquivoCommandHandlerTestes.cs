using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ImportacaoInscricao
{
    public class AlterarSituacaoRegistroImportacaoArquivoCommandHandlerTestes
    {
        private readonly Mock<IRepositorioImportacaoArquivoRegistro> _repositorioImportacaoRegistroMock;
        private readonly Faker _faker;
        private readonly AlterarSituacaoRegistroImportacaoArquivoCommandHandler _handler;

        public AlterarSituacaoRegistroImportacaoArquivoCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioImportacaoRegistroMock = mocker.GetMock<IRepositorioImportacaoArquivoRegistro>();
            _handler = mocker.CreateInstance<AlterarSituacaoRegistroImportacaoArquivoCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRegistroNaoLocalizado_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var comando = new AlterarSituacaoRegistroImportacaoArquivoCommand(_faker.Random.Long(1, 1000), _faker.PickRandom<SituacaoImportacaoArquivoRegistro>());
            
            _repositorioImportacaoRegistroMock.Setup(r => r.ObterPorId(comando.RegistroImportacaoId))
                .ReturnsAsync((ImportacaoArquivoRegistro)null);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));

            excecao.Message.Should().Be(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutarHandle_DeveAtualizarSituacaoERetornarTrue()
        {
            // Arrange
            var comando = new AlterarSituacaoRegistroImportacaoArquivoCommand(_faker.Random.Long(1, 1000), _faker.PickRandom<SituacaoImportacaoArquivoRegistro>());
            var importacaoRegistro = new ImportacaoArquivoRegistro();
            
            _repositorioImportacaoRegistroMock.Setup(r => r.ObterPorId(comando.RegistroImportacaoId))
                .ReturnsAsync(importacaoRegistro);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioImportacaoRegistroMock.Verify(r => r.Atualizar(It.IsAny<ImportacaoArquivoRegistro>()), Times.Once);
        }
    }
}
