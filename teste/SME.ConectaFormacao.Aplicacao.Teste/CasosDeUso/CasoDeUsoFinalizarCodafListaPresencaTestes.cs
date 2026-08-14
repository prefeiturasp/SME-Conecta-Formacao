using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoFinalizarCodafListaPresencaTestes
    {
        private const long CodafListaPresencaId = 1;
        private const string LoginUsuario = "1234567";

        private readonly Mock<IRepositorioCodafListaPresenca> repositorioCodafListaPresenca;
        private readonly Mock<IContextoAplicacao> contextoAplicacao;

        private readonly CasoDeUsoFinalizarCodafListaPresenca casoDeUso;

        public CasoDeUsoFinalizarCodafListaPresencaTestes()
        {
            repositorioCodafListaPresenca = new Mock<IRepositorioCodafListaPresenca>();
            contextoAplicacao = new Mock<IContextoAplicacao>();

            casoDeUso = new CasoDeUsoFinalizarCodafListaPresenca(
                repositorioCodafListaPresenca.Object,
                contextoAplicacao.Object);
        }

        [Fact]
        public async Task Deve_Retornar_NaoEncontrado_Quando_Lista_De_Presenca_Nao_Existir()
        {
            // Arrange
            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync((CodafListaPresenca?)null);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.NaoEncontrado, resultado.TipoFalha);
            Assert.Single(resultado.MensagensErro);
            Assert.Contains(
                "Lista de presença não encontrada.",
                resultado.MensagensErro);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Usuario_Nao_Administrador_Nao_For_Criador_Da_Lista()
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.AguardandoDf,
                criadoLogin: "outro-usuario");

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Single(resultado.MensagensErro);
            Assert.Contains(
                "Você não tem permissão para finalizar esta lista de presença.",
                resultado.MensagensErro);

            Assert.Equal(
                StatusCodafListaPresenca.AguardandoDf,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Lista_Ja_Estiver_Finalizada()
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.Finalizado,
                LoginUsuario);

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Single(resultado.MensagensErro);
            Assert.Contains(
                "Não é possível finalizar uma lista de presença com a situação 'Finalizada'.",
                resultado.MensagensErro);

            Assert.True(lista.EstaFinalizado());
            Assert.Equal(
                StatusCodafListaPresenca.Finalizado,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_Quando_Existir_Inscricao_Aprovada()
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.AguardandoDf,
                LoginUsuario,
                false,
                true,
                null);

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);
            Assert.Single(resultado.MensagensErro);
            Assert.Contains(
                "Lista de presença só pode ser finalizada se não houver aprovações.",
                resultado.MensagensErro);

            Assert.False(lista.EstaFinalizado());
            Assert.Equal(
                StatusCodafListaPresenca.AguardandoDf,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Finalizar_Lista_Quando_Nao_Houver_Inscricoes()
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.AguardandoDf,
                LoginUsuario);

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(TipoFalha.Nenhuma, resultado.TipoFalha);
            Assert.Empty(resultado.MensagensErro);

            Assert.True(lista.EstaFinalizado());
            Assert.Equal(
                StatusCodafListaPresenca.Finalizado,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(null)]
        public async Task Deve_Finalizar_Lista_Quando_Inscricoes_Nao_Estiverem_Aprovadas(
            bool? aprovado)
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.AguardandoDf,
                LoginUsuario,
                aprovado);

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(false);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(TipoFalha.Nenhuma, resultado.TipoFalha);
            Assert.Empty(resultado.MensagensErro);

            Assert.True(lista.EstaFinalizado());
            Assert.Equal(
                StatusCodafListaPresenca.Finalizado,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Permitir_Administrador_Finalizar_Lista_Criada_Por_Outro_Usuario()
        {
            // Arrange
            var lista = CriarLista(
                StatusCodafListaPresenca.AguardandoDf,
                criadoLogin: "outro-usuario");

            contextoAplicacao
                .SetupGet(c => c.EhAdministrador)
                .Returns(true);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(LoginUsuario);

            repositorioCodafListaPresenca
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId))
                .ReturnsAsync(lista);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(CodafListaPresencaId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(TipoFalha.Nenhuma, resultado.TipoFalha);

            Assert.True(lista.EstaFinalizado());
            Assert.Equal(
                StatusCodafListaPresenca.Finalizado,
                lista.Status);

            repositorioCodafListaPresenca.Verify(
                r => r.ObterPorIdDetalhadoAsync(CodafListaPresencaId),
                Times.Once);
        }

        private static CodafListaPresenca CriarLista(
            StatusCodafListaPresenca status,
            string criadoLogin,
            params bool?[] aprovacoes)
        {
            var lista = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                status)
            {
                CriadoLogin = criadoLogin
            };

            foreach (var aprovado in aprovacoes)
            {
                lista.CodafInscricoes.Add(
                    new CodafInscricaoListaPresenca
                    {
                        Aprovado = aprovado
                    });
            }

            return lista;
        }
    }
}
