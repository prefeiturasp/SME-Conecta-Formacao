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
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoCriarCodafCursoNaoHomologadoTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> _repositorioCodafMock;
        private readonly Mock<ICodafCursoNaoHomologadoInscritosService> _inscritosServiceMock;
        private readonly Mock<IGerenciadorAnexosCodafCursoNaoHomologadoService> _anexoServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        
        private readonly CasoDeUsoCriarCodafCursoNaoHomologado _sut;
        private readonly Faker _faker;

        public CasoDeUsoCriarCodafCursoNaoHomologadoTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologado>();
            _inscritosServiceMock = mocker.GetMock<ICodafCursoNaoHomologadoInscritosService>();
            _anexoServiceMock = mocker.GetMock<IGerenciadorAnexosCodafCursoNaoHomologadoService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();

            var dependencias = new CodafCursoNaoHomologadoDependencias(
                _repositorioCodafMock.Object,
                _inscritosServiceMock.Object,
                _anexoServiceMock.Object,
                _mapperMock.Object,
                _transacaoMock.Object
            );

            mocker.Use(dependencias);
            _sut = mocker.CreateInstance<CasoDeUsoCriarCodafCursoNaoHomologado>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoChamarExecutar_EntaoDeveRetornarSucessoESalvarDados()
        {
            // Arrange
            var codafCursoNaoHomologadoCadastroDto = new CodafCursoNaoHomologadoCadastroDto
            {
                PropostaId = _faker.Random.Long(1, long.MaxValue),
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue),
                Observacao = _faker.Lorem.Sentence(),
                Inscritos = new List<CodafCursoNaoHomologadoInscritoSalvarDto>(),
                Anexos = new List<CodafAnexoSalvarDto>()
            };

            var idEsperado = _faker.Random.Long(1, long.MaxValue);
            var transacaoDbMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);
            _repositorioCodafMock.Setup(r => r.Inserir(It.IsAny<CodafCursoNaoHomologado>())).ReturnsAsync(idEsperado);

            _mapperMock.Setup(m => m.Map<List<CodafCursoNaoHomologadoInscricao>>(codafCursoNaoHomologadoCadastroDto.Inscritos))
                       .Returns(new List<CodafCursoNaoHomologadoInscricao>());

            _mapperMock.Setup(m => m.Map<List<CodafCursoNaoHomologadoAnexo>>(codafCursoNaoHomologadoCadastroDto.Anexos))
                       .Returns(new List<CodafCursoNaoHomologadoAnexo>());

            _mapperMock.Setup(m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(It.IsAny<CodafCursoNaoHomologado>()))
                       .Returns(new CodafCursoNaoHomologadoDetalhadoDto { Id = idEsperado });

            // Act
            var resultado = await _sut.ExecutarAsync(codafCursoNaoHomologadoCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Id.Should().Be(idEsperado);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            transacaoDbMock.Verify(t => t.Rollback(), Times.Never);
            _repositorioCodafMock.Verify(r => r.Inserir(It.IsAny<CodafCursoNaoHomologado>()), Times.Once);
            _inscritosServiceMock.Verify(s => s.SalvarInscritosAsync(It.IsAny<List<CodafCursoNaoHomologadoInscricao>>(), idEsperado), Times.Once);
            _anexoServiceMock.Verify(s => s.ProcessarAnexosAsync(idEsperado, It.IsAny<List<CodafCursoNaoHomologadoAnexo>>()), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoInserir_QuandoChamarExecutar_EntaoDeveRetornarErroInternoERollback()
        {
            // Arrange
            var codafCursoNaoHomologadoCadastroDto = new CodafCursoNaoHomologadoCadastroDto
            {
                PropostaId = _faker.Random.Long(1, long.MaxValue),
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue),
                Observacao = _faker.Lorem.Sentence()
            };

            var transacaoDbMock = new Mock<IDbTransaction>();

            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);
            _repositorioCodafMock.Setup(r => r.Inserir(It.IsAny<CodafCursoNaoHomologado>())).ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var resultado = await _sut.ExecutarAsync(codafCursoNaoHomologadoCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            resultado.MensagensErro.Should().Contain("Erro ao salvar o codaf.");

            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }
    }
}
