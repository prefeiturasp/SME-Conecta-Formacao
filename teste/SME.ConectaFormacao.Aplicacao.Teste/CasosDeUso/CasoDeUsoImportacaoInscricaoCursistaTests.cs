using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoImportacaoInscricaoCursistaTests
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoImportacaoInscricaoCursista _casoDeUso;

        public CasoDeUsoImportacaoInscricaoCursistaTests()
        {
            _mocker = new AutoMocker();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoImportacaoInscricaoCursista>();
        }

        [Fact]
        public async Task DadoArquivoNuloOuVazioQuandoExecutarDeveLancarNegocioException()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(x => x.Length).Returns(0);
            long propostaId = 1;

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _casoDeUso.Executar(arquivoMock.Object, propostaId));

            Assert.Equal(MensagemNegocio.ARQUIVO_VAZIO, excecao.Message);
        }

        [Fact]
        public async Task DadoArquivoComFormatoInvalidoQuandoExecutarDeveLancarNegocioException()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(x => x.Length).Returns(1024);
            arquivoMock.Setup(x => x.ContentType).Returns("application/pdf"); // Formato inválido
            arquivoMock.Setup(x => x.FileName).Returns("arquivo.pdf");
            long propostaId = 1;

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                _casoDeUso.Executar(arquivoMock.Object, propostaId));

            Assert.Equal(MensagemNegocio.SOMENTE_ARQUIVO_XLSX_SUPORTADO, excecao.Message);
        }

        [Fact]
        public async Task DadoArquivoValidoQuandoExecutarDeveProcessarImportacaoEPublicarNaFila()
        {
            // Arrange
            long propostaId = 123;
            long idImportacaoGerado = 999;
            var streamArquivo = new MemoryStream();

            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(x => x.Length).Returns(1024);
            arquivoMock.Setup(x => x.ContentType).Returns("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            arquivoMock.Setup(x => x.FileName).Returns("inscricoes.xlsx");
            arquivoMock.Setup(x => x.OpenReadStream()).Returns(streamArquivo);

            // Mock do retorno do Command de Inserção
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<InserirImportacaoArquivoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idImportacaoGerado);

            // Act
            var resultado = await _casoDeUso.Executar(arquivoMock.Object, propostaId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(idImportacaoGerado, resultado.EntidadeId);
            Assert.Equal(MensagemNegocio.ARQUIVO_IMPORTADO_COM_SUCESSO, resultado.Mensagem);

            // Verifica se o comando de inserir registro de importação foi chamado corretamente
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<InserirImportacaoArquivoCommand>(c =>
                    c.ImportacaoArquivo.PropostaId == propostaId &&
                    c.ImportacaoArquivo.Nome == "inscricoes.xlsx" &&
                    c.ImportacaoArquivo.Situacao == Dominio.Enumerados.SituacaoImportacaoArquivo.CarregamentoInicial
                ),
                It.IsAny<CancellationToken>()), Times.Once);

            // Verifica se o comando de inserir o conteúdo (stream) foi chamado com o ID gerado
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<InserirConteudoArquivoInscricaoCursistaCommand>(c =>
                    c.ImportacaoArquivoId == idImportacaoGerado &&
                    c.StreamArquivo != null
                ),
                It.IsAny<CancellationToken>()), Times.Once);

            // Verifica se a mensagem foi publicada no RabbitMQ para validação
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<PublicarNaFilaRabbitCommand>(c =>
                    c.Rota == RotasRabbit.RealizarImportacaoInscricaoCursistaValidar &&
                    ((ImportacaoArquivoDTO)c.Filtros).Id == idImportacaoGerado
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
