using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoFinalizarCodafSuplementarTestes
    {
        private const long CodafSuplementarId = 1;
        private const string LoginUsuario = "1234567";

        private readonly Mock<IRepositorioCodafSuplementar> repositorioCodafSuplementarMock;
        private readonly Mock<IContextoAplicacao> contextoAplicacaoMock;
        private readonly CasoDeUsoFinalizarCodafSuplementar casoDeUso;

        public CasoDeUsoFinalizarCodafSuplementarTestes()
        {
            repositorioCodafSuplementarMock = new Mock<IRepositorioCodafSuplementar>();
            contextoAplicacaoMock = new Mock<IContextoAplicacao>();

            casoDeUso = new CasoDeUsoFinalizarCodafSuplementar(
                repositorioCodafSuplementarMock.Object,
                contextoAplicacaoMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_codaf_nao_for_encontrado()
        {
            // Arrange
            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync((CodafSuplementar)null!);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.False(resultado.Sucesso);

            repositorioCodafSuplementarMock.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId),
                Times.Once);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_usuario_nao_for_administrador_e_nao_for_criador_do_codaf()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: "7654321");

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.False(resultado.Sucesso);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_permitir_administrador_finalizar_codaf_criado_por_outro_usuario()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: "OUTRO_USUARIO");

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(true);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(codaf.EstaFinalizado());

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(
                    It.Is<CodafSuplementar>(c => c == codaf && c.EstaFinalizado())),
                Times.Once);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_codaf_ja_estiver_finalizado()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Finalizado,
                criadoLogin: LoginUsuario);

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.False(resultado.Sucesso);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_existir_inscricao_aprovada()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: LoginUsuario);

            codaf.CodafInscricoes.Add(CriarInscricao(aprovado: true));

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.False(resultado.Sucesso);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_finalizar_quando_nao_existirem_inscricoes()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: LoginUsuario);

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(codaf.EstaFinalizado());

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(codaf),
                Times.Once);
        }

        [Fact]
        public async Task Deve_finalizar_quando_existirem_inscricoes_mas_nenhuma_estiver_aprovada()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: LoginUsuario);

            codaf.CodafInscricoes.Add(CriarInscricao(aprovado: false));
            codaf.CodafInscricoes.Add(CriarInscricao(aprovado: null));

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(codaf.EstaFinalizado());

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(codaf),
                Times.Once);
        }

        [Fact]
        public async Task Deve_finalizar_quando_usuario_restrito_for_o_criador_do_codaf()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: LoginUsuario);

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.True(codaf.EstaFinalizado());

            contextoAplicacaoMock.Verify(
                c => c.LoginUsuario,
                Times.AtLeastOnce);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(codaf),
                Times.Once);
        }

        [Fact]
        public async Task Deve_propagar_excecao_quando_repositorio_falhar_ao_obter_codaf()
        {
            // Arrange
            var excecaoEsperada = new InvalidOperationException("Erro banco");

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => casoDeUso.ExecutarAsync(CodafSuplementarId));

            // Assert
            Assert.Same(excecaoEsperada, excecao);

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_propagar_excecao_quando_repositorio_falhar_ao_atualizar_codaf()
        {
            // Arrange
            var codaf = CriarCodaf(
                status: StatusCodafSuplementar.Iniciado,
                criadoLogin: LoginUsuario);

            var excecaoEsperada = new InvalidOperationException("Erro ao atualizar");

            contextoAplicacaoMock
                .Setup(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacaoMock
                .Setup(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafSuplementarMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(codaf);

            repositorioCodafSuplementarMock
                .Setup(r => r.Atualizar(codaf))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => casoDeUso.ExecutarAsync(CodafSuplementarId));

            // Assert
            Assert.Same(excecaoEsperada, excecao);
            Assert.True(codaf.EstaFinalizado());

            repositorioCodafSuplementarMock.Verify(
                r => r.Atualizar(codaf),
                Times.Once);
        }

        private static CodafSuplementar CriarCodaf(
            StatusCodafSuplementar status,
            string criadoLogin)
        {
            var codaf = (CodafSuplementar?)Activator.CreateInstance(
                typeof(CodafSuplementar),
                nonPublic: true)
                ?? throw new InvalidOperationException(
                    "Não foi possível criar CodafSuplementar.");

            DefinirPropriedade(codaf, "CodafId", CodafSuplementarId);
            DefinirPropriedade(codaf, "Status", status);
            DefinirPropriedade(codaf, "CriadoLogin", criadoLogin);

            codaf.CodafInscricoes ??= new List<CodafSuplementarInscricao>();

            return codaf;
        }

        private static CodafSuplementarInscricao CriarInscricao(bool? aprovado)
        {
            var inscricao =
                (CodafSuplementarInscricao?)Activator.CreateInstance(
                    typeof(CodafSuplementarInscricao),
                    nonPublic: true)
                ?? throw new InvalidOperationException(
                    "Não foi possível criar CodafSuplementarInscricao.");

            DefinirPropriedade(inscricao, "Aprovado", aprovado);

            return inscricao;
        }

        private static void DefinirPropriedade(
            object objeto,
            string propriedade,
            object? valor)
        {
            var propertyInfo = objeto
                .GetType()
                .GetProperty(propriedade);

            if (propertyInfo is null)
                throw new InvalidOperationException(
                    $"Propriedade '{propriedade}' não encontrada em '{objeto.GetType().Name}'.");

            propertyInfo.SetValue(objeto, valor);
        }
    }
}
