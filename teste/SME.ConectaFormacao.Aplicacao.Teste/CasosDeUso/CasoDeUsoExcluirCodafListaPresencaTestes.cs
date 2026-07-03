using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoExcluirCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoExcluirCodafListaPresenca _casoDeUsoExcluirCodafListaPresenca;
        private readonly Faker _faker;

        public CasoDeUsoExcluirCodafListaPresencaTestes()
        {
            var mocker = new AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _casoDeUsoExcluirCodafListaPresenca = mocker.CreateInstance<CasoDeUsoExcluirCodafListaPresenca>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmaListaInexistente_QuandoExecutarAsync_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            var idLista = _faker.Random.Long(1);

            // Act
            var resultado = await _casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(idLista);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
            _repositorioCodafListaPresencaMock.Verify(r => r.ExcluirAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaQueNaoPodeSerExcluida_QuandoExecutarAsync_EntaoRetornaErroNegocio()
        {
            // Arrange
            var idLista = _faker.Random.Long(1);
            var listaPresenca = new CodafListaPresenca(1, 1, new(null, null, null, null, null, null, null), null);
            listaPresenca.Iniciar();
            listaPresenca.MarcarComoEnviadaParaDf();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(idLista))
                .ReturnsAsync(listaPresenca);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Guid.NewGuid());
            // Act
            var resultado = await _casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(idLista);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Essa lista não pode ser excluída.");
            _repositorioCodafListaPresencaMock.Verify(r => r.ExcluirAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoUmaListaComStatusIniciado_QuandoExecutarAsync_EntaoExcluiListaComSucesso()
        {
            // Arrange
            var idLista = _faker.Random.Long(1);
            var listaPresenca = new CodafListaPresenca(1, 1, new(null, null, null, null, null, null, null), null);
            listaPresenca.Iniciar();
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(idLista))
                .ReturnsAsync(listaPresenca);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Guid.NewGuid());

            // Act
            var resultado = await _casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(idLista);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            _repositorioCodafListaPresencaMock.Verify(r => r.ExcluirAsync(idLista), Times.Once);
        }

        [Fact]
        public async Task DadoPerfilRestrito_QuandoTentarExcluirListaDeOutroUsuario_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var idLista = _faker.Random.Long(1);
            var loginCriadoLista = _faker.Internet.UserName();
            var loginUsuarioAtual = _faker.Internet.UserName();

            var listaPresenca = new CodafListaPresenca(1, 1, new(null, null, null, null, null, null, null), null)
            {
                Id = idLista,
                CriadoLogin = loginCriadoLista
            };
            listaPresenca.Iniciar();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(idLista))
                .ReturnsAsync(listaPresenca);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginUsuarioAtual);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.PARECERISTA);

            // Act
            var resultado = await _casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(idLista);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Você não tem permissão para excluir esta lista de presença.");
            _repositorioCodafListaPresencaMock.Verify(r => r.ExcluirAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DadoListaComSituacaoFinalizado_QuandoTentarExcluir_EntaoDeveRetornarErroNegocio()
        {
            // Arrange
            var idLista = _faker.Random.Long(1);
            var loginCriadoLista = _faker.Internet.UserName();

            var listaPresenca = new CodafListaPresenca(1, 1, new(null, null, null, null, null, null, null), null)
            {
                Id = idLista,
                CriadoLogin = loginCriadoLista
            };
            listaPresenca.Iniciar();
            listaPresenca.MarcarComoEnviadaParaDf();
            listaPresenca.Finalizar();

            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(idLista))
                .ReturnsAsync(listaPresenca);
            _contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(loginCriadoLista);
            _contextoAplicacaoMock
                .Setup(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            // Act
            var resultado = await _casoDeUsoExcluirCodafListaPresenca.ExecutarAsync(idLista);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível excluir uma lista de presença com situação 'Finalizado'.");
            _repositorioCodafListaPresencaMock.Verify(r => r.ExcluirAsync(It.IsAny<long>()), Times.Never);
        }
    }
}