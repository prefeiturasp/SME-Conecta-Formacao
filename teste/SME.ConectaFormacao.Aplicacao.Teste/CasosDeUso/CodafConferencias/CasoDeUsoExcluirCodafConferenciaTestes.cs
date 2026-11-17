using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoExcluirCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioCodafMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoExcluirCodafConferencia _sut;
        private readonly Faker _faker;

        public CasoDeUsoExcluirCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _sut = mocker.CreateInstance<CasoDeUsoExcluirCodafConferencia>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Codaf não encontrado.");
        }

        [Fact]
        public async Task DadoCodafComStatusDiferenteDeIniciado_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var codaf = new CodafConferencia(1, 1, new DadosPublicacaoLista(null, null, null, null, 123, null, null))
            {
                CodafInscricoes = [new() { InscricaoId = 1, Participou = true }]
            };
            codaf.DefinirStatus();
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Apenas registros com situação 'Iniciado' podem ser excluídos.");
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var codaf = new CodafConferencia(1, 1) { CriadoLogin = "login.outro" };
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
        public async Task DadoCodafIniciadoEPermissaoValida_QuandoExecutar_EntaoDeveExcluirERetornarSucesso()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var codaf = new CodafConferencia(1, 1) { CriadoLogin = "login.atual" };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(false);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("login.atual");

            // Act
            var resultado = await _sut.ExecutarAsync(id);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafMock.Verify(r => r.ExcluirAsync(id), Times.Once);
        }
    }
}
