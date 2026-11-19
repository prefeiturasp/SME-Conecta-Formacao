using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoAtualizarCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioCodafMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<ICodafConferenciaInscritosService> _inscritosServiceMock;
        private readonly Mock<IGerenciadorAnexosCodafConferenciaService> _anexoServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoAtualizarCodafConferencia _sut;
        private readonly Faker _faker;

        public CasoDeUsoAtualizarCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _inscritosServiceMock = mocker.GetMock<ICodafConferenciaInscritosService>();
            _anexoServiceMock = mocker.GetMock<IGerenciadorAnexosCodafConferenciaService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            var dependencias = new CodafConferenciaDependencias(
                _repositorioCodafMock.Object,
                _repositorioPropostaMock.Object,
                _inscritosServiceMock.Object,
                _anexoServiceMock.Object,
                _mapperMock.Object,
                _transacaoMock.Object
            );

            mocker.Use(dependencias);
            _sut = mocker.CreateInstance<CasoDeUsoAtualizarCodafConferencia>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto();

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Codaf não encontrado.");
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto();
            var codaf = new CodafConferencia(1, 1) { CriadoLogin = "login.outro" };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(false);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("login.atual");

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Você não tem permissão para editar este Codaf.");
        }

        [Fact]
        public async Task DadoStatusFinalizado_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto();
            var codaf = new CodafConferencia(1, 1, new DadosPublicacaoLista(null, null, null, null, 123, null, null))
            {
                CodafInscricoes = [new() { InscricaoId = 1, Participou = true }]
            };
            codaf.DefinirStatus();
            codaf.Finalizar();
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível editar um Codaf com status 'Finalizado'.");
        }

        [Fact]
        public async Task DadoCertificadoEmitido_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto();
            var certificado = (CodafCertificado)Activator.CreateInstance(typeof(CodafCertificado), true)!;
            var codaf = new CodafConferencia(1, 1)
            {
                CodafCertificados = [certificado]
            };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível editar um Codaf com certificados emitidos.");
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveAtualizarERetornarSucesso()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto
            {
                CodigoCursoEol = 777,
                Observacao = "Nova obs",
                Inscritos = [new() { InscricaoId = 1, Participou = true }]
            };
            var codaf = new CodafConferencia(1, 2) { Id = id, CriadoLogin = "login.atual" };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(false);
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns("login.atual");

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaInscricao>>(dto.Inscritos))
                .Returns([new() { InscricaoId = 1, Participou = true }]);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaAnexo>>(dto.Anexos))
                .Returns([]);

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            _repositorioCodafMock.Verify(r => r.Atualizar(codaf), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoAoAtualizar_QuandoExecutar_EntaoDeveFazerRollbackERetornarErroInterno()
        {
            // Arrange
            var id = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto();
            var codaf = new CodafConferencia(1, 2) { Id = id };
            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _repositorioCodafMock
                .Setup(r => r.Atualizar(It.IsAny<CodafConferencia>()))
                .ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            resultado.MensagensErro.Should().Contain("Erro ao salvar CODAF");
            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}
