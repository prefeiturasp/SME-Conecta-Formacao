using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoDevolverParaCorrecaoCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioComentarioCodafListaPresenca> _repositorioComentarioCodafListaPresencaMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly CasoDeUsoDevolverParaCorrecaoCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoDevolverParaCorrecaoCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioComentarioCodafListaPresencaMock = mocker.GetMock<IRepositorioComentarioCodafListaPresenca>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoDevolverParaCorrecaoCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaIdZero_QuandoExecutar_DeveRetornarErroDeValidacao()
        {
            // Arrange
            long codafListaPresencaId = 0;
            string justificativa = _faker.Lorem.Sentence();

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoUmaJustificativaVazia_QuandoExecutar_DeveRetornarErroDeValidacao()
        {
            // Arrange
            long codafListaPresencaId = _faker.Random.Long(1);
            string justificativa = string.Empty;

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaInexistente_QuandoExecutar_DeveRetornarErroDeNaoEncontrado()
        {
            // Arrange
            long codafListaPresencaId = _faker.Random.Long(1);
            string justificativa = _faker.Lorem.Sentence();

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaComStatusInvalido_QuandoExecutar_NaoDeveExecutarAcao()
        {
            // Arrange
            long codafListaPresencaId = _faker.Random.Long(1);
            string justificativa = _faker.Lorem.Sentence();

            var codafListaPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null) { Id = codafListaPresencaId };
            codafListaPresenca.Iniciar();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(codafListaPresencaId))
                .ReturnsAsync(codafListaPresenca);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock
                .Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
            _repositorioComentarioCodafListaPresencaMock
                .Verify(r => r.Inserir(It.IsAny<CodafComentarioListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmCodafListaPresencaValido_QuandoExecutar_DeveDevolverParaAreaPromotora()
        {
            // Arrange
            long codafListaPresencaId = _faker.Random.Long(1);
            string justificativa = _faker.Lorem.Sentence();
            var codafListaPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null) { Id = codafListaPresencaId };
            codafListaPresenca.Iniciar();
            codafListaPresenca.MarcarComoEnviadaParaDf();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(codafListaPresencaId))
                .ReturnsAsync(codafListaPresenca);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);
            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafListaPresencaMock
                .Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c => c.Status == StatusCodafListaPresenca.DevolvidoParaCorrecao && c.Id == codafListaPresencaId))
                , Times.Once);
            _repositorioComentarioCodafListaPresencaMock
                .Verify(r => r.Inserir(It.Is<CodafComentarioListaPresenca>(cc => cc.CodafListaPresencaId == codafListaPresencaId && cc.Comentario == justificativa))
                , Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoAtualizar_QuandoExecutar_EntaoDeveRetornarErroInternoERolarbackTransacao()
        {
            // Arrange
            long codafListaPresencaId = _faker.Random.Long(1);
            string justificativa = _faker.Lorem.Sentence();
            var codafListaPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null) { Id = codafListaPresencaId };
            codafListaPresenca.Iniciar();
            codafListaPresenca.MarcarComoEnviadaParaDf();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(codafListaPresencaId))
                .ReturnsAsync(codafListaPresenca);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Atualizar(It.IsAny<CodafListaPresenca>()))
                .ThrowsAsync(new Exception("Erro ao atualizar"));
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaId, justificativa);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Never);
        }

    }
}