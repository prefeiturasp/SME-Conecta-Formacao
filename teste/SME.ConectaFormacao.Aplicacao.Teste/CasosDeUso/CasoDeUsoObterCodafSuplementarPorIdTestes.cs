using AutoMapper;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Reflection;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafSuplementarPorIdTestes
    {
        private readonly Mock<IRepositorioCodafSuplementar> repositorioMock;
        private readonly Mock<IServicoArmazenamento> servicoArmazenamentoMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly CasoDeUsoObterCodafSuplementarPorId casoDeUso;

        public CasoDeUsoObterCodafSuplementarPorIdTestes()
        {
            repositorioMock = new Mock<IRepositorioCodafSuplementar>();
            servicoArmazenamentoMock = new Mock<IServicoArmazenamento>();
            mapperMock = new Mock<IMapper>();

            casoDeUso = new CasoDeUsoObterCodafSuplementarPorId(
                repositorioMock.Object,
                servicoArmazenamentoMock.Object,
                mapperMock.Object);
        }

        [Fact]
        public async Task DadoIdInexistente_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            const long codafSuplementarId = 1;

            repositorioMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafSuplementarId))
                .ReturnsAsync((CodafSuplementar?)null);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafSuplementarId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.NotNull(resultado.MensagensErro);
            Assert.Contains(
                "Codaf Suplementar não encontrado.",
                resultado.MensagensErro);

            repositorioMock.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafSuplementarId),
                Times.Once);

            mapperMock.Verify(
                m => m.Map<CodafSuplementarDetalhadoDto>(It.IsAny<CodafSuplementar>()),
                Times.Never);

            servicoArmazenamentoMock.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoCodafSemCertificadosEAnexosNulos_DeveRetornarCertificadoNaoEmitido()
        {
            // Arrange
            const long codafSuplementarId = 2;

            var entidade = CriarCodafSuplementar(codafSuplementarId);
            DefinirCertificados(entidade, null);

            var dto = new CodafSuplementarDetalhadoDto
            {
                Id = codafSuplementarId,
                Anexos = null
            };

            repositorioMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafSuplementarId))
                .ReturnsAsync(entidade);

            mapperMock
                .Setup(m => m.Map<CodafSuplementarDetalhadoDto>(entidade))
                .Returns(dto);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);

            var dados = Assert.IsType<CodafSuplementarDetalhadoDto>(
                resultado.Dados);

            Assert.False(dados.CertificadoEmitido);
            Assert.Null(dados.Anexos);

            repositorioMock.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafSuplementarId),
                Times.Once);

            mapperMock.Verify(
                m => m.Map<CodafSuplementarDetalhadoDto>(entidade),
                Times.Once);

            servicoArmazenamentoMock.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoCodafComColecoesVazias_DeveRetornarCertificadoNaoEmitidoSemBuscarUrls()
        {
            // Arrange
            const long codafSuplementarId = 3;

            var entidade = CriarCodafSuplementar(codafSuplementarId);

            DefinirCertificados(
                entidade,
                []);

            var dto = new CodafSuplementarDetalhadoDto
            {
                Id = codafSuplementarId,
                Anexos = []
            };

            repositorioMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafSuplementarId))
                .ReturnsAsync(entidade);

            mapperMock
                .Setup(m => m.Map<CodafSuplementarDetalhadoDto>(entidade))
                .Returns(dto);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);

            var dados = Assert.IsType<CodafSuplementarDetalhadoDto>(
                resultado.Dados);

            Assert.False(dados.CertificadoEmitido);
            Assert.NotNull(dados.Anexos);
            Assert.Empty(dados.Anexos);

            servicoArmazenamentoMock.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoCodafComCertificadoEAnexos_DeveInformarCertificadoEmitidoEPreencherUrls()
        {
            // Arrange
            const long codafSuplementarId = 4;

            var primeiroArquivoCodigo =
                Guid.Parse("32981ec4-65de-4551-a9d2-2f9e5d82beb5");

            var segundoArquivoCodigo =
                Guid.Parse("51c15075-b081-42b0-a3d0-380a3c194e53");

            const string primeiraUrl =
                "https://armazenamento.test/primeiro-arquivo.pdf";

            const string segundaUrl =
                "https://armazenamento.test/segundo-arquivo.pdf";

            var entidade = CriarCodafSuplementar(codafSuplementarId);
            var certificado = CriarCertificado();

            DefinirCertificados(
                entidade,
                [certificado]);

            var primeiroAnexo = new CodafSuplementarAnexoDto
            {
                ArquivoCodigo = primeiroArquivoCodigo,
                NomeArquivo = "primeiro-arquivo.pdf",
                Extensao = ".pdf"
            };

            var segundoAnexo = new CodafSuplementarAnexoDto
            {
                ArquivoCodigo = segundoArquivoCodigo,
                NomeArquivo = "segundo-arquivo.pdf",
                Extensao = ".pdf"
            };

            var dto = new CodafSuplementarDetalhadoDto
            {
                Id = codafSuplementarId,
                Anexos =
            [
                primeiroAnexo,
                segundoAnexo
            ]
            };

            repositorioMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafSuplementarId))
                .ReturnsAsync(entidade);

            mapperMock
                .Setup(m => m.Map<CodafSuplementarDetalhadoDto>(entidade))
                .Returns(dto);

            servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(
                    primeiroArquivoCodigo.ToString(), false))
                .ReturnsAsync(primeiraUrl);

            servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(
                    segundoArquivoCodigo.ToString(), false))
                .ReturnsAsync(segundaUrl);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafSuplementarId);

            // Assert
            Assert.True(resultado.Sucesso);

            var dados = Assert.IsType<CodafSuplementarDetalhadoDto>(
                resultado.Dados);

            Assert.True(dados.CertificadoEmitido);
            Assert.NotNull(dados.Anexos);
            Assert.Equal(2, dados.Anexos.Count);

            Assert.Collection(
                dados.Anexos,
                anexo => Assert.Equal(primeiraUrl, anexo.UrlDownload),
                anexo => Assert.Equal(segundaUrl, anexo.UrlDownload));

            repositorioMock.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafSuplementarId),
                Times.Once);

            mapperMock.Verify(
                m => m.Map<CodafSuplementarDetalhadoDto>(entidade),
                Times.Once);

            servicoArmazenamentoMock.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(
                    primeiroArquivoCodigo.ToString(), false),
                Times.Once);

            servicoArmazenamentoMock.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(
                    segundoArquivoCodigo.ToString(), false),
                Times.Once);
        }

        private static CodafSuplementar CriarCodafSuplementar(long codafId)
        {
            return new CodafSuplementar(codafId);
        }

        private static CodafCertificado CriarCertificado()
        {
            return (CodafCertificado?)Activator.CreateInstance(
                       typeof(CodafCertificado),
                       nonPublic: true)
                   ?? throw new InvalidOperationException(
                       "Não foi possível criar o certificado para o teste.");
        }

        private static void DefinirCertificados(
            CodafSuplementar entidade,
            ICollection<CodafCertificado>? certificados)
        {
            var propriedade = typeof(CodafSuplementar).GetProperty(
                "CodafCertificados",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic) ?? throw new InvalidOperationException(
                    "A propriedade CodafCertificados não foi encontrada.");
            propriedade.SetValue(entidade, certificados);
        }
    }
}
