using Bogus;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos
{
    public class GerenciadorAnexosCodafConferenciaServiceTestes
    {
        private readonly Mock<IRepositorioCodafConferenciaAnexo> _repositorioMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly GerenciadorAnexosCodafConferenciaService _sut;
        private readonly Faker _faker;

        public GerenciadorAnexosCodafConferenciaServiceTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafConferenciaAnexo>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _sut = mocker.CreateInstance<GerenciadorAnexosCodafConferenciaService>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoNovosAnexos_QuandoProcessar_EntaoDeveAdicionarMoverERemoverAntigos()
        {
            // Arrange
            var codafConferenciaId = _faker.Random.Long(1, 100);
            var arquivoRemoverId = Guid.NewGuid();
            var arquivoManterId = Guid.NewGuid();
            var arquivoAdicionarId = Guid.NewGuid();

            var anexosAtuais = new List<CodafConferenciaAnexo>
            {
                new() { ArquivoCodigo = arquivoRemoverId, NomeArquivo = "a", Extensao = "pdf" },
                new() { ArquivoCodigo = arquivoManterId, NomeArquivo = "b", Extensao = "pdf" }
            };

            var novosAnexos = new List<CodafConferenciaAnexo>
            {
                new() { ArquivoCodigo = arquivoManterId, NomeArquivo = "b", Extensao = "pdf" },
                new() { ArquivoCodigo = arquivoAdicionarId, NomeArquivo = "teste.pdf", Extensao = "pdf" }
            };

            _repositorioMock.Setup(r => r.ObterPorCodafConferenciaIdAsync(codafConferenciaId))
                            .ReturnsAsync(anexosAtuais);

            _repositorioMock.Setup(r => r.Remover(It.IsAny<CodafConferenciaAnexo>())).Returns(Task.CompletedTask);
            _repositorioMock.Setup(r => r.Inserir(It.IsAny<CodafConferenciaAnexo>())).ReturnsAsync(1L);
            _servicoArmazenamentoMock.Setup(s => s.MoverGuid(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());

            // Act
            await _sut.ProcessarAnexosAsync(codafConferenciaId, novosAnexos);

            // Assert
            _repositorioMock.Verify(r => r.Remover(It.Is<CodafConferenciaAnexo>(a => a.ArquivoCodigo == arquivoRemoverId)), Times.Once);

            _repositorioMock.Verify(r => r.Inserir(It.Is<CodafConferenciaAnexo>(a =>
                a.ArquivoCodigo == arquivoAdicionarId &&
                a.CodafConferenciaId == codafConferenciaId &&
                a.NomeArquivo == "teste.pdf" &&
                a.Extensao == ".pdf"
            )), Times.Once);

            _servicoArmazenamentoMock.Verify(s => s.MoverGuid(arquivoAdicionarId), Times.Once);

            _repositorioMock.Verify(r => r.Remover(It.Is<CodafConferenciaAnexo>(a => a.ArquivoCodigo == arquivoManterId)), Times.Never);
            _repositorioMock.Verify(r => r.Inserir(It.Is<CodafConferenciaAnexo>(a => a.ArquivoCodigo == arquivoManterId)), Times.Never);
        }

        [Fact]
        public async Task DadoNovosAnexosNulo_QuandoProcessar_EntaoDeveRemoverTodosOsAntigos()
        {
            // Arrange
            var codafConferenciaId = _faker.Random.Long(1, 100);
            var arquivoRemoverId = Guid.NewGuid();

            var anexosAtuais = new List<CodafConferenciaAnexo>
            {
                new() { ArquivoCodigo = arquivoRemoverId, NomeArquivo = "a", Extensao = "pdf" }
            };

            _repositorioMock.Setup(r => r.ObterPorCodafConferenciaIdAsync(codafConferenciaId))
                            .ReturnsAsync(anexosAtuais);

            _repositorioMock.Setup(r => r.Remover(It.IsAny<CodafConferenciaAnexo>())).Returns(Task.CompletedTask);

            // Act
            await _sut.ProcessarAnexosAsync(codafConferenciaId, null!);

            // Assert
            _repositorioMock.Verify(r => r.Remover(It.Is<CodafConferenciaAnexo>(a => a.ArquivoCodigo == arquivoRemoverId)), Times.Once);
            _repositorioMock.Verify(r => r.Inserir(It.IsAny<CodafConferenciaAnexo>()), Times.Never);
            _servicoArmazenamentoMock.Verify(s => s.MoverGuid(It.IsAny<Guid>()), Times.Never);
        }
    }
}
