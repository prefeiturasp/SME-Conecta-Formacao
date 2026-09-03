using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoExcluirCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioCodafMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoExcluirCodafCursoNaoHomologado _sut;
        private readonly Faker _faker;

        public CasoDeUsoExcluirCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _sut = mocker.CreateInstance<CasoDeUsoExcluirCodafCursoNaoHomologado>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoCodafFinalizado_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs")
            {
                CodafAnexos = [new() { ArquivoCodigo = Guid.NewGuid(), Extensao = "pdf", NomeArquivo = "arquivo.pdf" }],
                CodafInscricoes = [new()]
            };
            codaf.DefinirStatus();
            codaf.Finalizar();
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Codaf finalizado não pode ser excluído.");
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs") { CriadoLogin = "login.outro" };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(false);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("login.atual");

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Você não tem permissão para excluir este Codaf.");
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveExcluirERetornarSucesso()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs");
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafMock.Verify(r => r.ExcluirAsync(id), Times.Once);
        }
    }
}
