using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoCriarCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoCriarCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoCriarCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _mapperMock = mocker.GetMock<IMapper>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoCriarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoPropostaIdInvalido_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Proposta não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaTurmaIdInvalido_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdInvalido
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta());

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Proposta Turma não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaTurmaIdDiferenteDaProposta_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta());
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaIdValido))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaIdValido + 1 });
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("A turma não pertence à formação informada");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoTurmaJaPossuiListaDePresenca_QuandoExecutar_EntaoDeveRetornarErroNegocioENaoChamarInserir()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var turmaNome = _faker.Random.Word();
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta() { Id = propostaIdValido });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaIdValido))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaIdValido, Nome = turmaNome, Id = propostaTurmaIdValido });
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain($"A turma {turmaNome} já possui uma lista de presença cadastrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveChamarInserirERetornarSucesso()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaCadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta() { Id = propostaIdValido });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaIdValido))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaIdValido, Id = propostaTurmaIdValido });
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.Inserir(It.IsAny<CodafListaPresenca>()))
                .ReturnsAsync(1L);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(It.IsAny<CodafListaPresenca>()))
                .Returns(new CodafListaPresencaDto());

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaCadastroDto);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            _repositorioCodafListaPresencaMock.Verify(r => r.Inserir(It.Is<CodafListaPresenca>(c =>
                c.PropostaId == propostaIdValido &&
                c.PropostaTurmaId == propostaTurmaIdValido
            )), Times.Once);
        }
    }
}