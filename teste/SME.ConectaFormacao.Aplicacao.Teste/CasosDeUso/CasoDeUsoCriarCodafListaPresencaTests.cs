using AutoMapper;
using Bogus;
using FluentAssertions;
using FluentValidation;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoCriarCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafRetificacaoListaPresenca> _repositorioCodafRetificacaoListaPresencaMock;
        private readonly Mock<ICodafInscritosListaPresencaService> _inscritosServiceMock;
        private readonly Mock<IValidadorCodafListaPresencaService> _validadorDominioMock;
        private readonly Mock<IGerenciadorAnexosCodafService> _anexosServiceMock;
        private readonly Mock<IGerenciadorMovimentacaoCodafService> _movimentacaoServiceMock;

        private readonly Mock<IValidator<CodafListaPresencaCadastroDto>> _validatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ITransacao> _transacaoMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoCriarCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoCriarCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafRetificacaoListaPresencaMock = mocker.GetMock<IRepositorioCodafRetificacaoListaPresenca>();
            _inscritosServiceMock = mocker.GetMock<ICodafInscritosListaPresencaService>();
            _validadorDominioMock = mocker.GetMock<IValidadorCodafListaPresencaService>();
            _anexosServiceMock = mocker.GetMock<IGerenciadorAnexosCodafService>();
            _movimentacaoServiceMock = mocker.GetMock<IGerenciadorMovimentacaoCodafService>();
            var dependencias = new CodafListaPresencaDependencias(
                _repositorioCodafListaPresencaMock.Object,
                _repositorioCodafRetificacaoListaPresencaMock.Object,
                _inscritosServiceMock.Object,
                _validadorDominioMock.Object,
                _anexosServiceMock.Object,
                _movimentacaoServiceMock.Object
            );
            mocker.Use(dependencias);
            _validatorMock = mocker.GetMock<IValidator<CodafListaPresencaCadastroDto>>();
            _mapperMock = mocker.GetMock<IMapper>();
            _transacaoMock = mocker.GetMock<ITransacao>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoCriarCodafListaPresenca>();
            _faker = new();
            
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
        }

        [Fact]
        public async Task DadoUmDtoInvalido_QuandoChamarExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                [
                    new FluentValidation.Results.ValidationFailure("PropostaId", "PropostaId inválido."),
                    new FluentValidation.Results.ValidationFailure("PropostaTurmaId", "PropostaTurmaId inválido.")
                ]));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("PropostaId inválido.");
            resultado.MensagensErro.Should().Contain("PropostaTurmaId inválido.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);

        }

        [Fact]
        public async Task DadoErroDeVinculo_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };
            var mensagemErroVinculo = _faker.Lorem.Sentence();

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorDominioMock
                .Setup(v => v.ValidarVinculoPropostaTurmaAsync(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(Erro.Validacao(mensagemErroVinculo));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain(mensagemErroVinculo);
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroUnicidadeTurma_QuandoExecutar_EntaoDeveRetornarErroValidacao()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdInvalido
            };
            var mensagemErroVinculo = _faker.Lorem.Sentence();

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _validadorDominioMock
                .Setup(v => v.ValidarUnicidadeTurmaListaDePresencaAsync(It.IsAny<long>(), 0))
                .ReturnsAsync(Erro.Validacao(mensagemErroVinculo));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain(mensagemErroVinculo);
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
        }

        [Fact]
        public async Task DadoCriacaoSemInscritos_QuandoExecutar_EntaoDeveChamarInserirERetornarSucesso()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Inserir(It.IsAny<CodafListaPresenca>()))
                .ReturnsAsync(1L);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(It.IsAny<CodafListaPresenca>()))
                .Returns(new CodafListaPresencaDto());
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.Is<CodafListaPresenca>(c =>
                c.PropostaId == propostaIdValido &&
                c.PropostaTurmaId == propostaTurmaIdValido
            )), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Once);
            transacaoMock.Verify(t => t.Rollback(), Times.Never);
        }

        [Fact]
        public async Task DadoCriacaoComInscritos_QuandoExecutar_EntaoDeveChamarInserirInscritosERetornarSucesso()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1);
            var propostaTurmaIdValido = _faker.Random.Long(1);
            var inscritosDto = new List<CodafInscritoListaPresencaSalvarDto>
            {
                new () { InscricaoId = _faker.Random.Long(1) },
                new () { InscricaoId = _faker.Random.Long(1) }
            };
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Inscritos = inscritosDto
            };
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Inserir(It.IsAny<CodafListaPresenca>()))
                .ReturnsAsync(1L);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(It.IsAny<CodafListaPresenca>()))
                .Returns(new CodafListaPresencaDto());
            _mapperMock
                .Setup(m => m.Map<List<CodafInscricaoListaPresenca>>(inscritosDto))
                .Returns(
                [
                    new() { InscricaoId = inscritosDto[0].InscricaoId },
                    new() { InscricaoId = inscritosDto[1].InscricaoId }
                ]);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            Assert.NotNull(resultado);
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            _inscritosServiceMock.Verify(r => r.SalvarInscritosAsync(It.Is<List<CodafInscricaoListaPresenca>>(inscritos =>
                inscritos.Count == 2 &&
                inscritos.Any(i => i.InscricaoId == inscritosDto[0].InscricaoId) &&
                inscritos.Any(i => i.InscricaoId == inscritosDto[1].InscricaoId)), It.IsAny<long>()
                ), Times.Once);
        }

        [Fact]
        public async Task DadoCriacaoComRetificacoes_QuandoExecutar_EntaoDeveChamarInserirRetificacoesERetornarSucesso()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1);
            var propostaTurmaIdValido = _faker.Random.Long(1);
            var retificacoesDto = new List<CodafRetificacaoListaPresencaSalvarDto>
            {
                new () { DataRetificacao = _faker.Date.Past() },
                new () { DataRetificacao = _faker.Date.Recent() }
            };
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido,
                Retificacoes = retificacoesDto
            };
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Inserir(It.IsAny<CodafListaPresenca>()))
                .ReturnsAsync(1L);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(It.IsAny<CodafListaPresenca>()))
                .Returns(new CodafListaPresencaDto());
            _mapperMock
                .Setup(m => m.Map<IEnumerable<CodafRetificacaoListaPresenca>>(retificacoesDto))
                .Returns(
                [
                    new() { DataRetificacao = retificacoesDto[0].DataRetificacao },
                    new() { DataRetificacao = retificacoesDto[1].DataRetificacao }
                ]);
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Inserir(It.Is<CodafRetificacaoListaPresenca>(retificacao =>
                retificacao.DataRetificacao == retificacoesDto[0].DataRetificacao)), Times.Once);
            _repositorioCodafRetificacaoListaPresencaMock.Verify(r => r.Inserir(It.Is<CodafRetificacaoListaPresenca>(retificacao =>
                retificacao.DataRetificacao == retificacoesDto[1].DataRetificacao)), Times.Once);
        }

        [Fact]
        public async Task DadoErroAoInserir_QuandoExecutar_EntaoDeveRetornarErroInternoERollbackTransacao()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };
            _validatorMock
                .Setup(v => v.ValidateAsync(codafListaPresencaCadastroDto, default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Inserir(It.IsAny<CodafListaPresenca>()))
                .ThrowsAsync(new Exception("Erro ao inserir"));
            var transacaoMock = new Mock<IDbTransaction>();
            _transacaoMock
                .Setup(t => t.Iniciar())
                .Returns(transacaoMock.Object);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Erro ao salvar a lista de presença.");
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            transacaoMock.Verify(t => t.Rollback(), Times.Once);
            transacaoMock.Verify(t => t.Commit(), Times.Never);
        }
    }
}