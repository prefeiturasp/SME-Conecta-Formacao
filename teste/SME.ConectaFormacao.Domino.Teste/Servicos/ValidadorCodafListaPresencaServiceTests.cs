using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;

namespace SME.ConectaFormacao.Domino.Teste.Servicos
{
    public class ValidadorCodafListaPresencaServiceTests
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioListaMock;
        private readonly ValidadorCodafListaPresencaService _validadorService;
        private readonly Faker _faker;

        public ValidadorCodafListaPresencaServiceTests()
        {
            var mocker = new AutoMocker();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _repositorioListaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _validadorService = mocker.CreateInstance<ValidadorCodafListaPresencaService>();
            _faker = new("pt_BR");
        }

        [Fact]
        public async Task DadoUmaTurmaComListaDePresenca_QuandoValidarUnicidadeTurmaListaDePresenca_EntaoDeveRetornarErroDeNegocio()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioListaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, It.IsAny<long>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _validadorService.ValidarUnicidadeTurmaListaDePresencaAsync(propostaTurmaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.RegraDeNegocio);
            resultado.Value.Mensagens.Should().Contain("Já existe uma lista de presença cadastrada para esta turma.");
        }

        [Fact]
        public async Task DadoUmaTurmaSemListaDePresenca_QuandoValidarUnicidadeTurmaListaDePresenca_EntaoNaoDeveRetornarErro()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioListaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaId, It.IsAny<long>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _validadorService.ValidarUnicidadeTurmaListaDePresencaAsync(propostaTurmaId);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task DadoUmaPropostaInexistente_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("Proposta não encontrada.");
        }

        [Fact]
        public async Task DadoUmaTurmaInexistente_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta());

            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("Turma não encontrada.");
        }

        [Fact]
        public async Task DadoUmaTurmaDeOutraProposta_QuandoValidarVinculoPropostaTurma_EntaoDeveRetornarErroDeValidacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId + 1 });

            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Value.Tipo.Should().Be(TipoFalha.Validacao);
            resultado.Value.Mensagens.Should().Contain("A turma informada não pertence à formação selecionada.");
        }

        [Fact]
        public async Task DadoUmaTurmaValida_QuandoValidarVinculoPropostaTurma_EntaoNaoDeveRetornarErro()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaId))
                .ReturnsAsync(new Proposta { Id = propostaId });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaId))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaId });
            // Act
            var resultado = await _validadorService.ValidarVinculoPropostaTurmaAsync(propostaId, propostaTurmaId);
            // Assert
            resultado.Should().BeNull();
        }
    }
}
