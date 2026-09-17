using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Arquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Arquivos
{
    public class CasoDeUsoArquivoCarregarTemporarioTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoArquivoCarregarTemporario _casoDeUso;

        public CasoDeUsoArquivoCarregarTemporarioTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoArquivoCarregarTemporario>();
        }

        [Fact]
        public async Task DadoArquivoVazio_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act
            var acao = async () => await _casoDeUso.Executar(fileMock.Object);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.ARQUIVO_VAZIO);

            _mediatorMock.Verify(m => m.Send(It.IsAny<InserirArquivoCommand>(), default), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ArmazenarArquivoTemporarioServicoArmazenamentoCommand>(), default), Times.Never);
        }

        [Fact]
        public async Task DadoArquivoMaiorQue10MB_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10 * 1024 * 1024 + 1);

            // Act
            var acao = async () => await _casoDeUso.Executar(fileMock.Object);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.ARQUIVO_MAIOR_QUE_10_MB);

            _mediatorMock.Verify(m => m.Send(It.IsAny<InserirArquivoCommand>(), default), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ArmazenarArquivoTemporarioServicoArmazenamentoCommand>(), default), Times.Never);
        }

        [Fact]
        public async Task DadoArquivoValido_QuandoExecutar_EntaoDeveRetornarArquivoArmazenadoDTO()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1024);
            fileMock.Setup(f => f.FileName).Returns("teste.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");

            var idEsperado = 123L;
            var caminhoEsperado = "/temp/teste.pdf";

            _mediatorMock.Setup(m => m.Send(It.IsAny<InserirArquivoCommand>(), default))
                .ReturnsAsync(idEsperado);
            _mediatorMock.Setup(m => m.Send(It.IsAny<ArmazenarArquivoTemporarioServicoArmazenamentoCommand>(), default))
                .ReturnsAsync(caminhoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(fileMock.Object);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(idEsperado);
            resultado.Path.Should().Be(caminhoEsperado);
            resultado.Codigo.Should().NotBeEmpty();

            _mediatorMock.Verify(m => m.Send(It.Is<InserirArquivoCommand>(c => 
                c.Arquivo.Nome == "teste.pdf" &&
                c.Arquivo.TipoConteudo == "application/pdf" &&
                c.Arquivo.FormFile == fileMock.Object), default), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<ArmazenarArquivoTemporarioServicoArmazenamentoCommand>(c => 
                c.Arquivo.Nome == "teste.pdf" &&
                c.Arquivo.TipoConteudo == "application/pdf" &&
                c.Arquivo.FormFile == fileMock.Object), default), Times.Once);
        }
    }
}
