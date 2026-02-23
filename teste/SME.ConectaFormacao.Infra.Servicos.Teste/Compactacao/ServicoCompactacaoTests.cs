using Bogus;
using SME.ConectaFormacao.Infra.Servicos.Compactacao;

namespace SME.ConectaFormacao.Infra.Servicos.Teste.Compactacao
{
    public class ServicoCompactacaoTests
    {
        [Fact]
        public async Task DadoArquivosValidos_QuandoCompactarAsync_EntaoDeveRetornarVetorDeBytesNaoVazio()
        {
            // Arrange
            var faker = new Faker("pt_BR");
            var servico = new ServicoCompactacao();

            using var conteudoFake = new MemoryStream(faker.Random.Bytes(150));
            var arquivos = new List<ArquivoCompactacaoDto>
            {
                new($"certificado_{faker.Random.Number(1000, 9999)}.pdf", conteudoFake)
            };

            // Act
            var resultado = await servico.CompactarAssincronamenteAsync(arquivos, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Length > 0);
        }
    }
}
