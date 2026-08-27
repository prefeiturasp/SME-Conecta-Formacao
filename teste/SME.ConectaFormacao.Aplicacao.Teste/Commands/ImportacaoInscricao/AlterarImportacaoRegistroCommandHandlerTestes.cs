using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.ImportacaoInscricao
{
    public class AlterarImportacaoRegistroCommandHandlerTestes
    {
        private readonly Mock<IRepositorioImportacaoArquivoRegistro> _repositorioImportacaoArquivoRegistroMock;
        private readonly Faker _faker;
        private readonly AlterarImportacaoRegistroCommandHandler _handler;

        public AlterarImportacaoRegistroCommandHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioImportacaoArquivoRegistroMock = mocker.GetMock<IRepositorioImportacaoArquivoRegistro>();
            _handler = mocker.CreateInstance<AlterarImportacaoRegistroCommandHandler>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoRegistroNaoLocalizado_QuandoExecutarHandle_DeveLancarExcecaoDeNegocio()
        {
            // Arrange
            var dto = new AlterarImportacaoRegistroDto(
                _faker.Random.Long(1, 1000), _faker.Lorem.Text(), SituacaoImportacaoArquivoRegistro.Processado, null);
            var comando = new AlterarImportacaoRegistroCommand(dto);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _handler.Handle(comando, CancellationToken.None));

            excecao.Message.Should().Be(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);
        }

        [Fact]
        public async Task DadoRegistroValido_QuandoExecutarHandle_DeveAtualizarERetornarTrue()
        {
            // Arrange
            var dto = new AlterarImportacaoRegistroDto(_faker.Random.Long(1, 1000), _faker.Lorem.Text(), SituacaoImportacaoArquivoRegistro.Erro, _faker.Lorem.Sentence());
            var comando = new AlterarImportacaoRegistroCommand(dto);

            var entidade = new ImportacaoArquivoRegistro();

            _repositorioImportacaoArquivoRegistroMock.Setup(r => r.ObterPorId(dto.Id))
                .ReturnsAsync(entidade);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            _repositorioImportacaoArquivoRegistroMock.Verify(r => r.Atualizar(It.Is<ImportacaoArquivoRegistro>(i => 
                i.Situacao == dto.Situacao &&
                i.Conteudo == dto.Conteudo &&
                i.Erro == dto.Erro
            )), Times.Once);
        }
    }
}
