using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarParaDfCodafListaPresencaTests
    {
        private readonly Mock<IValidadorCodafListaPresencaService> _validadorCodafListaPresencaServiceMock;
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly CasoDeUsoEnviarParaDfCodafListaPresenca _casoDeUsoEnviarParaDfCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoEnviarParaDfCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _validadorCodafListaPresencaServiceMock = mocker.GetMock<IValidadorCodafListaPresencaService>();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _casoDeUsoEnviarParaDfCodafListaPresenca = mocker.CreateInstance<CasoDeUsoEnviarParaDfCodafListaPresenca>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmaListaInexistente_QuandoExecutar_EntaoDeveRetornarNaoEncontrado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);

            // Act
            var resultado = await _casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoUmaListaInvalida_QuandoExecutar_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null);
            codafListPresenca.Iniciar();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListPresenca);

            _validadorCodafListaPresencaServiceMock
                .Setup(v => v.ValidarParaEnvioAoDfAsync(codafListPresenca))
                .ReturnsAsync(Erro.Negocio(""));

            // Act
            var resultado = await _casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(codafListPresenca.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
        }

        [Fact]
        public async Task DadoUmaListaQueNaoPodeSerEnviadaParaDf_QuandoExecutar_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var codafListPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListPresenca);

            // Act
            var resultado = await _casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(codafListPresenca.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaValida_QuandoExecutar_EntaoDeveRetornarSucesso()
        {
            // Arrange
            var codafListPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null);
            codafListPresenca.Iniciar();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListPresenca);
            _validadorCodafListaPresencaServiceMock
                .Setup(v => v.ValidarParaEnvioAoDfAsync(codafListPresenca))
                .ReturnsAsync((Erro?)null);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(codafListPresenca.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafListaPresencaMock
                .Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c => c.Id == codafListPresenca.Id && c.Status == StatusCodafListaPresenca.AguardandoDf))
                , Times.Once);
        }


        [Fact]
        public async Task DadoErroAoAtualizar_QuandoExecutar_EntaoDeveRetornarErroInternoERollbackTransacao()
        {
            // Arrange
            var codafListPresenca = new CodafListaPresenca(1, 1, null, null, null, null, null, null, null, null);
            codafListPresenca.Iniciar();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(codafListPresenca);
            _validadorCodafListaPresencaServiceMock
                .Setup(v => v.ValidarParaEnvioAoDfAsync(codafListPresenca))
                .ReturnsAsync((Erro?)null);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Atualizar(It.IsAny<CodafListaPresenca>()))
                .ThrowsAsync(new Exception("Erro ao atualizar"));

            // Act
            var resultado = await _casoDeUsoEnviarParaDfCodafListaPresenca.ExecutarAsync(codafListPresenca.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}
