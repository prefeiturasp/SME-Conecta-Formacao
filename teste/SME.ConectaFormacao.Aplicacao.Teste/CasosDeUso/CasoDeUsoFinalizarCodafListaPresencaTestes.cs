using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoFinalizarCodafListaPresencaTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoFinalizarCodafListaPresenca _casoDeUso;

        public CasoDeUsoFinalizarCodafListaPresencaTestes()
        {
            _repositorioCodafListaPresencaMock = new Mock<IRepositorioCodafListaPresenca>();
            _contextoAplicacaoMock = new Mock<IContextoAplicacao>();

            _casoDeUso = new CasoDeUsoFinalizarCodafListaPresenca(
                _repositorioCodafListaPresencaMock.Object,
                _contextoAplicacaoMock.Object);
        }

        [Fact]
        public async Task DadoUmaListaInexistente_QuandoExecutarAsync_EntaoRetornaErroNaoEncontrado()
        {
            // Arrange
            const long listaId = 1;

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync((CodafListaPresenca?)null);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.NaoEncontrado, resultado.TipoFalha);
            Assert.Contains(
                "Lista de presença não encontrada.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoPerfilRestritoEListaDeOutroUsuario_QuandoExecutarAsync_EntaoRetornaErroNegocio()
        {
            // Arrange
            const long listaId = 1;
            const string loginCriador = "criador";
            const string loginUsuarioAtual = "outro.usuario";

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.Iniciado,
                loginCriador);

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.PARECERISTA);

            _contextoAplicacaoMock
                .Setup(x => x.LoginUsuario)
                .Returns(loginUsuarioAtual);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Contains(
                "Você não tem permissão para finalizar esta lista de presença.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoPerfilRestritoECriadorDaLista_QuandoExecutarAsync_EntaoFinalizaComSucesso()
        {
            // Arrange
            const long listaId = 1;
            const string loginUsuario = "usuario";

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.Iniciado,
                loginUsuario);

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.PARECERISTA);

            _contextoAplicacaoMock
                .Setup(x => x.LoginUsuario)
                .Returns(loginUsuario);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.FinalizarAsync(listaId))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.True(resultado.Sucesso);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(listaId),
                Times.Once);
        }

        [Fact]
        public async Task DadoListaFinalizada_QuandoExecutarAsync_EntaoRetornaErroNegocio()
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.Finalizado,
                "usuario");

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Contains(
                "Não é possível finalizar uma lista de presença com a situação 'Finalizada'.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoListaQueNaoPodeSerFinalizadaPeloPerfil_QuandoExecutarAsync_EntaoRetornaErroNegocio()
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.AguardandoDf,
                "usuario");

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.EMFORPEF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Contains(
                "Essa lista não pode ser finalizada.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoListaComInscritoAprovado_QuandoExecutarAsync_EntaoNaoDeveFinalizar()
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.AguardandoDf,
                "usuario",
                true);

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.NaoEncontrado, resultado.TipoFalha);
            Assert.Contains(
                "Lista de presença só pode ser finalizada se não houver aprovações.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoListaComInscritosAprovadosENaoAprovados_QuandoExecutarAsync_EntaoNaoDeveFinalizar()
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.AguardandoDf,
                "usuario",
                false,
                true,
                null);

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Contains(
                "Lista de presença só pode ser finalizada se não houver aprovações.",
                resultado.MensagensErro);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoListaSemInscritos_QuandoExecutarAsync_EntaoFinalizaComSucesso()
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.AguardandoDf,
                "usuario");

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.FinalizarAsync(listaId))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.True(resultado.Sucesso);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(listaId),
                Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public async Task DadoListaComInscritoNaoAprovado_QuandoExecutarAsync_EntaoFinalizaComSucesso(
            bool? aprovado)
        {
            // Arrange
            const long listaId = 1;

            var lista = CriarLista(
                listaId,
                StatusCodafListaPresenca.AguardandoDf,
                "usuario",
                aprovado);

            _contextoAplicacaoMock
                .Setup(x => x.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.ObterPorIdDetalhadoAsync(listaId))
                .ReturnsAsync(lista);

            _repositorioCodafListaPresencaMock
                .Setup(x => x.FinalizarAsync(listaId))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.ExecutarAsync(listaId);

            // Assert
            Assert.True(resultado.Sucesso);

            _repositorioCodafListaPresencaMock.Verify(
                x => x.FinalizarAsync(listaId),
                Times.Once);
        }

        private static CodafListaPresenca CriarLista(
            long listaId,
            StatusCodafListaPresenca status,
            string criadoLogin,
            params bool?[] aprovacoes)
        {
            var lista = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                status)
            {
                Id = listaId,
                CriadoLogin = criadoLogin
            };

            foreach (var aprovado in aprovacoes)
            {
                lista.CodafInscricoes.Add(
                    new CodafInscricaoListaPresenca
                    {
                        CodafListaPresencaId = listaId,
                        Aprovado = aprovado
                    });
            }

            return lista;
        }
    }
}
