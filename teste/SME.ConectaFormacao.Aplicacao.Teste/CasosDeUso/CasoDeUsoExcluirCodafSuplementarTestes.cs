using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoExcluirCodafSuplementarTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> repositorioMock;
        private readonly CasoDeUsoExcluirCodafSuplementar casoDeUso;

        public CasoDeUsoExcluirCodafSuplementarTestes()
        {
            repositorioMock = new Mock<IRepositorioCodafSuplementar>();

            casoDeUso = new CasoDeUsoExcluirCodafSuplementar(
                repositorioMock.Object);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_codaf_suplementar_nao_for_encontrado()
        {
            const long id = 1;

            repositorioMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(id))
                .ReturnsAsync((CodafSuplementar)null!);

            var resultado = await casoDeUso.ExecutarAsync(id);

            Assert.False(resultado.Sucesso);

            repositorioMock.Verify(
                r => r.ObterNaoExcluidosPorIdAsync(id),
                Times.Once);

            repositorioMock.Verify(
                r => r.ExcluirAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_codaf_suplementar_estiver_finalizado()
        {
            const long id = 1;

            var entidade = CodafSuplementarBuilder.Criar();
            entidade.GetType().GetProperty("Status")?.SetValue(entidade, StatusCodafSuplementar.Finalizado);

            repositorioMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(id))
                .ReturnsAsync(entidade);

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().Contain("Não é possível excluir um Codaf suplementar com situação 'Finalizado'.");

            repositorioMock.Verify(
                r => r.ExcluirAsync(It.IsAny<long>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_excluir_codaf_suplementar_quando_encontrado()
        {
            const long id = 1;

            var entidade = CodafSuplementarBuilder.Criar(); 

            repositorioMock
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(id))
                .ReturnsAsync(entidade);

            repositorioMock
                .Setup(r => r.ExcluirAsync(id))
                .Returns(Task.CompletedTask);

            var resultado = await casoDeUso.ExecutarAsync(id);

            Assert.True(resultado.Sucesso);

            repositorioMock.Verify(
                r => r.ObterNaoExcluidosPorIdAsync(id),
                Times.Once);

            repositorioMock.Verify(
                r => r.ExcluirAsync(id),
                Times.Once);
        }
    }

    internal static class CodafSuplementarBuilder
    {
        public static CodafSuplementar Criar()
        {
            var entidade = (CodafSuplementar?)Activator.CreateInstance(
                typeof(CodafSuplementar),
                nonPublic: true
            ) ?? throw new InvalidOperationException("Não foi possível criar uma instância de CodafSuplementar.");
            entidade.GetType().GetProperty("CodafId")?.SetValue(entidade, 1L);
            entidade.GetType().GetProperty("Status")?.SetValue(entidade, StatusCodafSuplementar.Iniciado);
            return entidade;
        }
    }
}
