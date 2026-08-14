using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioCodafMock;
        private readonly Mock<ICodafCursoNaoHomologadoInscritosService> _inscritosServiceMock;
        private readonly Mock<IGerenciadorAnexosCodafCursoNaoHomologadoService> _anexoServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoAtualizarCodafCursoNaoHomologado _sut;
        private readonly Faker _faker;

        public CasoDeUsoAtualizarCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _inscritosServiceMock = mocker.GetMock<ICodafCursoNaoHomologadoInscritosService>();
            _anexoServiceMock = mocker.GetMock<IGerenciadorAnexosCodafCursoNaoHomologadoService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            var dependencias = new CodafCursoNaoHomologadoDependencias(
                _repositorioCodafMock.Object,
                _inscritosServiceMock.Object,
                _anexoServiceMock.Object,
                _mapperMock.Object,
                _transacaoMock.Object
            );

            mocker.Use(dependencias);
            _sut = mocker.CreateInstance<CasoDeUsoAtualizarCodafCursoNaoHomologado>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoCodafNaoEncontrado_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var dto = new CodafCursoNaoHomologadoCadastroDto();

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDiferente_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var dto = new CodafCursoNaoHomologadoCadastroDto();
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs") { CriadoLogin = "login.outro" };
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
            var id = _faker.Random.Long(1, long.MaxValue);
            var dto = new CodafCursoNaoHomologadoCadastroDto();
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
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível editar um Codaf com status 'Finalizado'.");
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveSalvarERetornarSucesso()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var dto = new CodafCursoNaoHomologadoCadastroDto
            {
                Observacao = _faker.Lorem.Sentence(),
                Inscritos = [],
                Anexos = []
            };
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs Anterior");

            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _mapperMock
                .Setup(m => m.Map<List<CodafCursoNaoHomologadoAnexo>>(It.IsAny<List<CodafAnexoSalvarDto>>()))
                .Returns([]);
            _mapperMock
                .Setup(m => m.Map<List<CodafCursoNaoHomologadoInscricao>>(It.IsAny<List<CodafCursoNaoHomologadoInscritoSalvarDto>>()))
                .Returns([]);

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafMock.Verify(r => r.Atualizar(codaf), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoSalvar_QuandoExecutar_EntaoDeveRetornarErroInternoERollback()
        {
            // Arrange
            var id = _faker.Random.Long(1, long.MaxValue);
            var dto = new CodafCursoNaoHomologadoCadastroDto();
            var codaf = new CodafCursoNaoHomologado(1, 1, "Obs");

            _repositorioCodafMock.Setup(r => r.ObterNaoExcluidosPorIdAsync(id)).ReturnsAsync(codaf);
            _contextoAplicacaoMock.Setup(c => c.EhAdministrador).Returns(true);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _repositorioCodafMock.Setup(r => r.Atualizar(It.IsAny<CodafCursoNaoHomologado>())).ThrowsAsync(new Exception());

            // Act
            var resultado = await _sut.ExecutarAsync(dto, id);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
        }
    }
}
