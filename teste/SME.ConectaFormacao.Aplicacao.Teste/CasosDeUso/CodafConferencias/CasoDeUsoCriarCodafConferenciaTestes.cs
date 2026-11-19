using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CasoDeUsoCriarCodafConferenciaTestes
    {
        private readonly Mock<IRepositorioCodafConferencia> _repositorioCodafMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<ICodafConferenciaInscritosService> _inscritosServiceMock;
        private readonly Mock<IGerenciadorAnexosCodafConferenciaService> _anexoServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly CasoDeUsoCriarCodafConferencia _sut;
        private readonly Faker _faker;

        public CasoDeUsoCriarCodafConferenciaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafMock = mocker.GetMock<IRepositorioCodafConferencia>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _inscritosServiceMock = mocker.GetMock<ICodafConferenciaInscritosService>();
            _anexoServiceMock = mocker.GetMock<IGerenciadorAnexosCodafConferenciaService>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();

            var dependencias = new CodafConferenciaDependencias(
                _repositorioCodafMock.Object,
                _repositorioPropostaMock.Object,
                _inscritosServiceMock.Object,
                _anexoServiceMock.Object,
                _mapperMock.Object,
                _transacaoMock.Object
            );

            mocker.Use(dependencias);
            _sut = mocker.CreateInstance<CasoDeUsoCriarCodafConferencia>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutar_EntaoDeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync((Proposta?)null);

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            resultado.MensagensErro.Should().Contain("Proposta não encontrada.");
        }

        [Fact]
        public async Task DadoPropostaNaoConferencia_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta { Id = dto.PropostaId, TipoFormacao = TipoFormacao.Curso });

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("A proposta informada não é do tipo Conferência.");
        }

        [Fact]
        public async Task DadoPropostaNaoPublicada_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta
                {
                    Id = dto.PropostaId,
                    TipoFormacao = TipoFormacao.Conferencia,
                    Situacao = SituacaoProposta.Rascunho
                });

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("A proposta informada deve estar com a situação Publicada.");
        }

        [Fact]
        public async Task DadoTurmaJaPossuiCodaf_QuandoExecutar_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta
                {
                    Id = dto.PropostaId,
                    TipoFormacao = TipoFormacao.Conferencia,
                    Situacao = SituacaoProposta.Publicada
                });

            _repositorioCodafMock
                .Setup(r => r.TurmaJaTemCodafAsync(dto.PropostaTurmaId, null))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.MensagensErro.Should().Contain("Já existe um CODAF cadastrado para esta turma.");
        }

        [Fact]
        public async Task DadoCadastroCompleto_QuandoExecutar_EntaoDeveGravarComStatusAguardando()
        {
            // Arrange
            var idGerado = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = 10,
                PropostaTurmaId = 20,
                CodigoCursoEol = 555,
                CodigoNivel = 2,
                NumeroComunicado = 100,
                PaginaComunicadoDom = 15,
                DataComunicado = DateTime.Today,
                DataPublicacaoDom = DateTime.Today,
                Observacao = "Obs teste",
                Inscritos = [new() { InscricaoId = 1, Participou = true }],
                Anexos = [new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = "doc.pdf" }]
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta { Id = dto.PropostaId, TipoFormacao = TipoFormacao.Conferencia, Situacao = SituacaoProposta.Publicada });

            _repositorioCodafMock
                .Setup(r => r.TurmaJaTemCodafAsync(dto.PropostaTurmaId, null))
                .ReturnsAsync(false);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _repositorioCodafMock
                .Setup(r => r.Inserir(It.IsAny<CodafConferencia>()))
                .ReturnsAsync(idGerado);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaInscricao>>(dto.Inscritos))
                .Returns([new() { InscricaoId = 1, Participou = true }]);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaAnexo>>(dto.Anexos))
                .Returns([new() { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = "doc.pdf", Extensao = ".pdf" }]);

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Id.Should().Be(idGerado);
            resultado.Dados.Status.Should().Be(StatusCodafConferencia.Aguardando);
            resultado.Dados.PropostaId.Should().Be(dto.PropostaId);
            resultado.Dados.PropostaTurmaId.Should().Be(dto.PropostaTurmaId);
            resultado.Dados.CodigoFormacao.Should().Be(dto.PropostaId);
            resultado.Dados.CodigoCursoEol.Should().Be(dto.CodigoCursoEol);
            resultado.Dados.CodigoNivel.Should().Be(dto.CodigoNivel);
            resultado.Dados.NumeroComunicado.Should().Be(dto.NumeroComunicado);
            resultado.Dados.PaginaComunicadoDom.Should().Be(dto.PaginaComunicadoDom);
            resultado.Dados.DataComunicado.Should().Be(dto.DataComunicado);
            resultado.Dados.DataPublicacaoDom.Should().Be(dto.DataPublicacaoDom);
            resultado.Dados.Observacao.Should().Be(dto.Observacao);

            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
            _repositorioCodafMock.Verify(r => r.Atualizar(It.Is<CodafConferencia>(c => c.Status == StatusCodafConferencia.Aguardando)), Times.Once);
        }

        [Fact]
        public async Task DadoCadastroIncompleto_QuandoExecutar_EntaoDeveGravarComStatusIniciado()
        {
            // Arrange
            var idGerado = _faker.Random.Long(1, 1000);
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = 10,
                PropostaTurmaId = 20,
                CodigoCursoEol = null,
                Inscritos = []
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta { Id = dto.PropostaId, TipoFormacao = TipoFormacao.Conferencia, Situacao = SituacaoProposta.Publicada });

            _repositorioCodafMock
                .Setup(r => r.TurmaJaTemCodafAsync(dto.PropostaTurmaId, null))
                .ReturnsAsync(false);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _repositorioCodafMock
                .Setup(r => r.Inserir(It.IsAny<CodafConferencia>()))
                .ReturnsAsync(idGerado);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaInscricao>>(dto.Inscritos))
                .Returns([]);

            _mapperMock
                .Setup(m => m.Map<List<CodafConferenciaAnexo>>(dto.Anexos))
                .Returns([]);

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados!.Status.Should().Be(StatusCodafConferencia.Iniciado);
            transacaoDbMock.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoAoInserir_QuandoExecutar_EntaoDeveFazerRollbackERetornarErroInterno()
        {
            // Arrange
            var dto = new CodafConferenciaCadastroDto
            {
                PropostaId = 10,
                PropostaTurmaId = 20
            };

            _repositorioPropostaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(dto.PropostaId))
                .ReturnsAsync(new Proposta { Id = dto.PropostaId, TipoFormacao = TipoFormacao.Conferencia, Situacao = SituacaoProposta.Publicada });

            _repositorioCodafMock
                .Setup(r => r.TurmaJaTemCodafAsync(dto.PropostaTurmaId, null))
                .ReturnsAsync(false);

            var transacaoDbMock = new Mock<IDbTransaction>();
            _transacaoMock.Setup(t => t.Iniciar()).Returns(transacaoDbMock.Object);

            _repositorioCodafMock
                .Setup(r => r.Inserir(It.IsAny<CodafConferencia>()))
                .ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var resultado = await _sut.ExecutarAsync(dto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            resultado.MensagensErro.Should().Contain("Erro ao salvar o codaf.");
            transacaoDbMock.Verify(t => t.Rollback(), Times.Once);
            transacaoDbMock.Verify(t => t.Commit(), Times.Never);
        }
    }
}
