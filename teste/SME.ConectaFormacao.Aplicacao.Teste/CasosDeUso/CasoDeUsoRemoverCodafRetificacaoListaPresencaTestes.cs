using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoRemoverCodafRetificacaoListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafRetificacaoListaPresenca> _repositorioCodafRetificacaoListaPresencaMock;
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoRemoverCodafRetificacaoListaPresenca _casoDeUsoRemoverCodafRetificacaoListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoRemoverCodafRetificacaoListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafRetificacaoListaPresencaMock = mocker.GetMock<IRepositorioCodafRetificacaoListaPresenca>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _casoDeUsoRemoverCodafRetificacaoListaPresenca = mocker.CreateInstance<CasoDeUsoRemoverCodafRetificacaoListaPresenca>();
            _faker = new();

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
        }

        [Fact]
        public async Task DadoRetificacaoExistente_QuandoExecutar_EntaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            var criadoLogin = _faker.Internet.UserName();

            var retificacao = new CodafRetificacaoListaPresenca
            {
                Id = retificacaoId,
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1),
                CodafListaPresencaId = _faker.Random.Long(1)
            };

            var codaf = new CodafListaPresenca(
                propostaId: _faker.Random.Long(1),
                propostaTurmaId: _faker.Random.Long(1),
                StatusCodafListaPresenca.Iniciado)
            {
                CriadoLogin = criadoLogin
            };

            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await _casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock
                .Verify(r => r.Remover(retificacao), Times.Once);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
        }

        [Fact]
        public async Task DadoRetificacaoInexistente_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);

            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync((CodafRetificacaoListaPresenca?)null);

            // Act
            var resultado = await _casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock
                .Verify(r => r.Remover(It.IsAny<CodafRetificacaoListaPresenca>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Retificação não encontrada.");
        }

        [Fact]
        public async Task DadoListaPresencaInexistente_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);

            var retificacao = new CodafRetificacaoListaPresenca
            {
                Id = retificacaoId,
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1),
                CodafListaPresencaId = _faker.Random.Long(1)
            };

            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId))
                .ReturnsAsync((CodafListaPresenca?)null);

            // Act
            var resultado = await _casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock
                .Verify(r => r.Remover(It.IsAny<CodafRetificacaoListaPresenca>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
        }

        [Fact]
        public async Task DadoCodafFinalizadoComPerfilRestrito_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            var criadoLogin = _faker.Internet.UserName();
            var loginOutroUsuario = _faker.Internet.UserName();

            var retificacao = new CodafRetificacaoListaPresenca
            {
                Id = retificacaoId,
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1),
                CodafListaPresencaId = _faker.Random.Long(1)
            };

            var codaf = new CodafListaPresenca(
                propostaId: _faker.Random.Long(1),
                propostaTurmaId: _faker.Random.Long(1),
                StatusCodafListaPresenca.Finalizado)
            {
                CriadoLogin = criadoLogin
            };

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns(loginOutroUsuario);

            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await _casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock
                .Verify(r => r.Remover(It.IsAny<CodafRetificacaoListaPresenca>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível remover retificações de um CODAF criado por outro usuário.");
        }

        [Fact]
        public async Task DadoCodafFinalizado_QuandoExecutar_EntaoNaoDeveChamarRemover()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            var criadoLogin = _faker.Internet.UserName();

            var retificacao = new CodafRetificacaoListaPresenca
            {
                Id = retificacaoId,
                DataRetificacao = _faker.Date.Past(),
                PaginaRetificacaoDom = _faker.Random.Short(1),
                CodafListaPresencaId = _faker.Random.Long(1)
            };

            var codaf = new CodafListaPresenca(
                propostaId: _faker.Random.Long(1),
                propostaTurmaId: _faker.Random.Long(1),
                StatusCodafListaPresenca.Finalizado)
            {
                CriadoLogin = criadoLogin
            };

            _repositorioCodafRetificacaoListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacaoId))
                .ReturnsAsync(retificacao);

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(retificacao.CodafListaPresencaId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await _casoDeUsoRemoverCodafRetificacaoListaPresenca.ExecutarAsync(retificacaoId);

            // Assert
            _repositorioCodafRetificacaoListaPresencaMock
                .Verify(r => r.Remover(It.IsAny<CodafRetificacaoListaPresenca>()), Times.Never);

            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível remover retificações de uma lista de presença com situação 'Finalizado'.");
        }
    }
}
