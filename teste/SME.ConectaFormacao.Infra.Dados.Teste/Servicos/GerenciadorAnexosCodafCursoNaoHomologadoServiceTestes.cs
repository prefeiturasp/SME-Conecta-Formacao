using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos
{
    public class GerenciadorAnexosCodafCursoNaoHomologadoServiceTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologadoAnexo> _repositorioMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly GerenciadorAnexosCodafCursoNaoHomologadoService _sut;
        private readonly Faker _faker;

        public GerenciadorAnexosCodafCursoNaoHomologadoServiceTestes()
        {
            var mocker = new AutoMocker();
            _repositorioMock = mocker.GetMock<IRepositorioCodafCursoNaoHomologadoAnexo>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _sut = mocker.CreateInstance<GerenciadorAnexosCodafCursoNaoHomologadoService>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoNovosAnexos_QuandoProcessar_EntaoDeveAdicionarMoverERemoverAntigos()
        {
            // Arrange
            var codafCursoNaoHomologadoId = _faker.Random.Long(1, 100);
            var arquivoRemoverId = Guid.NewGuid();
            var arquivoManterId = Guid.NewGuid();
            var arquivoAdicionarId = Guid.NewGuid();

            var anexosAtuais = new List<CodafCursoNaoHomologadoAnexo>
            {
                new CodafCursoNaoHomologadoAnexo { ArquivoCodigo = arquivoRemoverId, NomeArquivo = "a", Extensao = "pdf" },
                new CodafCursoNaoHomologadoAnexo { ArquivoCodigo = arquivoManterId, NomeArquivo = "b", Extensao = "pdf" }
            };

            var novosAnexos = new List<CodafCursoNaoHomologadoAnexo>
            {
                new CodafCursoNaoHomologadoAnexo { ArquivoCodigo = arquivoManterId, NomeArquivo = "b", Extensao = "pdf" },
                new CodafCursoNaoHomologadoAnexo { ArquivoCodigo = arquivoAdicionarId, NomeArquivo = "teste.pdf", Extensao = "pdf" }
            };

            _repositorioMock.Setup(r => r.ObterPorCodafCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId))
                            .ReturnsAsync(anexosAtuais);
            
            _repositorioMock.Setup(r => r.Remover(It.IsAny<CodafCursoNaoHomologadoAnexo>())).Returns(Task.CompletedTask);
            _repositorioMock.Setup(r => r.Inserir(It.IsAny<CodafCursoNaoHomologadoAnexo>())).ReturnsAsync(1L);
            _servicoArmazenamentoMock.Setup(s => s.MoverGuid(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());

            // Act
            await _sut.ProcessarAnexosAsync(codafCursoNaoHomologadoId, novosAnexos);

            // Assert
            _repositorioMock.Verify(r => r.Remover(It.Is<CodafCursoNaoHomologadoAnexo>(a => a.ArquivoCodigo == arquivoRemoverId)), Times.Once);
            
            _repositorioMock.Verify(r => r.Inserir(It.Is<CodafCursoNaoHomologadoAnexo>(a => 
                a.ArquivoCodigo == arquivoAdicionarId && 
                a.CodafCursoNaoHomologadoId == codafCursoNaoHomologadoId &&
                a.NomeArquivo == "teste.pdf" &&
                a.Extensao == ".pdf"
            )), Times.Once);

            _servicoArmazenamentoMock.Verify(s => s.MoverGuid(arquivoAdicionarId), Times.Once);

            // Verifica que o arquivo mantido não foi nem removido nem inserido
            _repositorioMock.Verify(r => r.Remover(It.Is<CodafCursoNaoHomologadoAnexo>(a => a.ArquivoCodigo == arquivoManterId)), Times.Never);
            _repositorioMock.Verify(r => r.Inserir(It.Is<CodafCursoNaoHomologadoAnexo>(a => a.ArquivoCodigo == arquivoManterId)), Times.Never);
        }

        [Fact]
        public async Task DadoNovosAnexosNulo_QuandoProcessar_EntaoDeveRemoverTodosOsAntigos()
        {
            // Arrange
            var codafCursoNaoHomologadoId = _faker.Random.Long(1, 100);
            var arquivoRemoverId = Guid.NewGuid();

            var anexosAtuais = new List<CodafCursoNaoHomologadoAnexo>
            {
                new CodafCursoNaoHomologadoAnexo { ArquivoCodigo = arquivoRemoverId, NomeArquivo = "a", Extensao = "pdf" }
            };

            _repositorioMock.Setup(r => r.ObterPorCodafCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId))
                            .ReturnsAsync(anexosAtuais);
            
            _repositorioMock.Setup(r => r.Remover(It.IsAny<CodafCursoNaoHomologadoAnexo>())).Returns(Task.CompletedTask);

            // Act
            await _sut.ProcessarAnexosAsync(codafCursoNaoHomologadoId, null);

            // Assert
            _repositorioMock.Verify(r => r.Remover(It.Is<CodafCursoNaoHomologadoAnexo>(a => a.ArquivoCodigo == arquivoRemoverId)), Times.Once);
            _repositorioMock.Verify(r => r.Inserir(It.IsAny<CodafCursoNaoHomologadoAnexo>()), Times.Never);
            _servicoArmazenamentoMock.Verify(s => s.MoverGuid(It.IsAny<Guid>()), Times.Never);
        }
    }
}
