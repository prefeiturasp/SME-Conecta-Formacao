using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoRemoverCodafSuplementarRetificacaoTestes
    {
        private readonly Mock<IRepositorioCodafSuplementarRetificacao> _repositorioCodafSuplementarRetificacaoMock;
        private readonly Mock<IRepositorioCodafSuplementar> _repositorioCodafSuplementarMock;
        private readonly CasoDeUsoRemoverCodafSuplementarRetificacao _casoDeUsoRemoverCodafSuplementarRetificacao;
        private readonly Faker _faker;

        public CasoDeUsoRemoverCodafSuplementarRetificacaoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafSuplementarRetificacaoMock = mocker.GetMock<IRepositorioCodafSuplementarRetificacao>();
            _repositorioCodafSuplementarMock = mocker.GetMock<IRepositorioCodafSuplementar>();
            _casoDeUsoRemoverCodafSuplementarRetificacao = mocker.CreateInstance<CasoDeUsoRemoverCodafSuplementarRetificacao>();
            _faker = new();
        }

        [Fact]
        public async Task DadoRetificacaoInexistente_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);

            _repositorioCodafSuplementarRetificacaoMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync((CodafSuplementarRetificacao?)null);

            // Act
            var resultado = await _casoDeUsoRemoverCodafSuplementarRetificacao.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafSuplementarRetificacaoMock
                .Verify(r => r.Remover(It.IsAny<CodafSuplementarRetificacao>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Retificação não encontrada.");
        }

        [Fact]
        public async Task DadoCodafSuplementarInexistente_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            var retificacao = new CodafSuplementarRetificacao
            {
                Id = retificacaoId,
                CodafSuplementarId = _faker.Random.Long(1),
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1)
            };

            _repositorioCodafSuplementarRetificacaoMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafSuplementarMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacao.CodafSuplementarId))
                .ReturnsAsync((CodafSuplementar?)null);

            // Act
            var resultado = await _casoDeUsoRemoverCodafSuplementarRetificacao.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafSuplementarRetificacaoMock
                .Verify(r => r.Remover(It.IsAny<CodafSuplementarRetificacao>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Codaf suplementar não encontrada.");
        }

        [Fact]
        public async Task DadoRetificacaoExistenteECodafSuplementarExistente_QuandoExecutar_EntaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            var codafSuplementarId = _faker.Random.Long(1);

            var retificacao = new CodafSuplementarRetificacao
            {
                Id = retificacaoId,
                CodafSuplementarId = codafSuplementarId,
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1)
            };

            var codafSuplementar = new CodafSuplementar(_faker.Random.Long(1));

            _repositorioCodafSuplementarRetificacaoMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafSuplementarMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(codafSuplementarId))
                .ReturnsAsync(codafSuplementar);

            // Act
            var resultado = await _casoDeUsoRemoverCodafSuplementarRetificacao.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafSuplementarRetificacaoMock
                .Verify(r => r.Remover(retificacao), Times.Once);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }
    }
}