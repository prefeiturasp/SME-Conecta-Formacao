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
    public class CasoDeUsoAtualizarCodafListaPresencaTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly CasoDeUsoAtualizarCodafListaPresenca _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoAtualizarCodafListaPresencaTests()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAtualizarCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoCodafListPresencaInexistente_QuandoExecutar_EntaoDeveErroNaoEncontradoENaoChamarAtualizar()
        {
            // Arrange
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = _faker.Random.Long(1, long.MaxValue),
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Int(1, int.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.NaoEncontrado);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaIdInvalido_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarAtualizar()
        {
            // Arrange
            var propostaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdInvalido,
                PropostaTurmaId = _faker.Random.Long(1, long.MaxValue)
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdInvalido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Proposta não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaTurmaIdInvalido_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarAtualizar()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdInvalido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdInvalido
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta());

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Proposta Turma não encontrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaTurmaIdDiferenteDaProposta_QuandoExecutar_EntaoDeveRetornarErroValidacaoENaoChamarInserir()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta());
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaIdValido))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaIdValido + 1 });

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("A turma não pertence à formação informada");
            resultado.TipoFalha.Should().Be(TipoFalha.Validacao);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoTurmaJaPossuiListaDePresenca_QuandoExecutar_EntaoDeveRetornarErroNegocioENaoChamarAtualizar()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var turmaNome = _faker.Random.Word();
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = propostaIdValido,
                PropostaTurmaId = propostaTurmaIdValido
            };

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido + 1, codafListaPresencaEdicaoDto.PropostaTurmaId, null, null, null, null, null, null, null, null));
            _repositorioPropostaMock
                .Setup(r => r.ObterPorId(propostaIdValido))
                .ReturnsAsync(new Proposta() { Id = propostaIdValido });
            _repositorioPropostaMock
                .Setup(r => r.ObterTurmaPorId(propostaTurmaIdValido))
                .ReturnsAsync(new PropostaTurma { PropostaId = propostaIdValido, Nome = turmaNome, Id = propostaTurmaIdValido });
            _repositorioCodafListaPresencaMock
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido, It.IsAny<long>()))
                .ReturnsAsync(true);
            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain($"A turma {turmaNome} já possui uma lista de presença cadastrada.");
            resultado.TipoFalha.Should().Be(TipoFalha.RegraDeNegocio);
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.IsAny<CodafListaPresenca>()), Times.Never);
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoExecutar_EntaoDeveChamarAtualizarUmaVez()
        {
            // Arrange
            var propostaIdValido = _faker.Random.Long(1, long.MaxValue);
            var propostaTurmaIdValido = _faker.Random.Long(1, long.MaxValue);
            var codafListaPresencaEdicaoDto = new CodafListaPresencaEdicaoDto
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
                .Setup(r => r.TurmaJaTemListaDePresencaAsync(propostaTurmaIdValido, It.IsAny<long>()))
                .ReturnsAsync(false);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorId(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca(propostaIdValido, propostaTurmaIdValido, null, null, null, null, null, null, null, null));

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(codafListaPresencaEdicaoDto, _faker.Random.Long(1, long.MaxValue));

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.Atualizar(It.Is<CodafListaPresenca>(c =>
                c.PropostaId == propostaIdValido &&
                c.PropostaTurmaId == propostaTurmaIdValido
            )), Times.Once);
        }
    }
}
