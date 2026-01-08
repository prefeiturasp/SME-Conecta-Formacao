using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Webapi.Controllers;
using System.Net;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class CodafListaPresencaControllerTests
    {
        private readonly Mock<ICasoDeUsoCriarCodafListaPresenca> _mockCasoDeUsoCriar;
        private readonly Mock<ICasoDeUsoAtualizarCodafListaPresenca> _mockCasoDeUsoAtualizar;
        private readonly Mock<ICasoDeUsoListarCodafListaPresenca> _mockCasoDeUsoListar;
        private readonly Mock<ICasoDeUsoObterCodafListaPresencaPorId> _mockCasoDeUsoObterPorId;
        private readonly Mock<ICasoDeUsoListarInscritosTurmaCodafListaPresenca> _mockCasoDeUsoListarInscritosTurma;
        private readonly Mock<ICasoDeUsoTurmaPossuiCodafListaPresenca> _mockCasoDeUsoTurmaPossuiCodafListaPresenca;
        private readonly Mock<ICasoDeUsoRemoverCodafRetificacaoListaPresenca> _mockCasoDeUsoRemoverRetificacao;
        private readonly Mock<ICasoDeUsoObterModeloTermoResponsabilidadeCodaf> _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf;
        private readonly Mock<ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca> _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca;
        private readonly Mock<ICasoDeUsoEnviarParaDfCodafListaPresenca> _mockCasoDeUsoEnviarParaDfCodafListaPresenca;
        private readonly Mock<ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca> _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca;
        private readonly CodafListaPresencaController _controller;
        private readonly Faker _faker;

        public CodafListaPresencaControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoCriar = mocker.GetMock<ICasoDeUsoCriarCodafListaPresenca>();
            _mockCasoDeUsoAtualizar = mocker.GetMock<ICasoDeUsoAtualizarCodafListaPresenca>();
            _mockCasoDeUsoListar = mocker.GetMock<ICasoDeUsoListarCodafListaPresenca>();
            _mockCasoDeUsoObterPorId = mocker.GetMock<ICasoDeUsoObterCodafListaPresencaPorId>();
            _mockCasoDeUsoListarInscritosTurma = mocker.GetMock<ICasoDeUsoListarInscritosTurmaCodafListaPresenca>();
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca = mocker.GetMock<ICasoDeUsoTurmaPossuiCodafListaPresenca>();
            _mockCasoDeUsoRemoverRetificacao = mocker.GetMock<ICasoDeUsoRemoverCodafRetificacaoListaPresenca>();
            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf = mocker.GetMock<ICasoDeUsoObterModeloTermoResponsabilidadeCodaf>();
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca = mocker.GetMock<ICasoDeUsoUploadAnexoTemporarioCodafListaPresenca>();
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca = mocker.GetMock<ICasoDeUsoEnviarParaDfCodafListaPresenca>();
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca = mocker.GetMock<ICasoDeUsoDevolverParaCorrecaoCodafListaPresenca>();
            _controller = mocker.CreateInstance<CodafListaPresencaController>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCadastroValido_QuandoCadastrar_EntaoDeveChamarCasoDeUsoCriar()
        {
            // Arrange
            var cadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };

            var codafDto = new CodafListaPresencaDto
            {
                PropostaId = cadastroDto.PropostaId,
                PropostaTurmaId = cadastroDto.PropostaTurmaId
            };

            _mockCasoDeUsoCriar
                .Setup(x => x.ExecutarAsync(cadastroDto))
                .ReturnsAsync(Resultado<CodafListaPresencaDto>.DeSucesso(codafDto));

            // Act
            await _controller.Cadastrar(cadastroDto);

            // Assert
            _mockCasoDeUsoCriar.Verify(x => x.ExecutarAsync(cadastroDto), Times.Once);
        }

        [Fact]
        public async Task DadoCadastroValido_QuandoCadastrar_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var cadastroDto = new CodafListaPresencaCadastroDto
            {
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };
            var codafDto = new CodafListaPresencaDto
            {
                PropostaId = cadastroDto.PropostaId,
                PropostaTurmaId = cadastroDto.PropostaTurmaId
            };
            _mockCasoDeUsoCriar
                .Setup(x => x.ExecutarAsync(cadastroDto))
                .ReturnsAsync(Resultado<CodafListaPresencaDto>.DeSucesso(codafDto));

            // Act
            var resultado = await _controller.Cadastrar(cadastroDto) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.Created);
            var resultadoValor = resultado.Value as CodafListaPresencaDto;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(codafDto);
        }

        [Fact]
        public async Task DadoAtualizacaoValida_QuandoAtualizar_EntaoDeveChamarCasoDeUsoAtualizar()
        {
            // Arrange
            var id = _faker.Random.Int(1);
            var edicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };
            _mockCasoDeUsoAtualizar
                .Setup(x => x.ExecutarAsync(edicaoDto, id))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            await _controller.Atualizar(id, edicaoDto);

            // Assert
            _mockCasoDeUsoAtualizar.Verify(x => x.ExecutarAsync(edicaoDto, id), Times.Once);
        }

        [Fact]
        public async Task DadoAtualizacaoValida_QuandoAtualizar_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var id = _faker.Random.Int(1);
            var edicaoDto = new CodafListaPresencaEdicaoDto
            {
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };
            _mockCasoDeUsoAtualizar
                .Setup(x => x.ExecutarAsync(edicaoDto, id))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            var resultado = await _controller.Atualizar(id, edicaoDto) as StatusCodeResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterPorId_EntaoDeveChamarCasoDeUsoObterPorId()
        {
            // Arrange
            var id = _faker.Random.Long(1);
            var codafDto = new CodafListaPresencaDto
            {
                Id = id,
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };
            _mockCasoDeUsoObterPorId
                .Setup(x => x.ExecutarAsync(id))
                .ReturnsAsync(Resultado<CodafListaPresencaDto>.DeSucesso(codafDto));
            // Act
            await _controller.ObterPorId(id);
            // Assert
            _mockCasoDeUsoObterPorId.Verify(x => x.ExecutarAsync(id), Times.Once);
        }

        [Fact]
        public async Task DadoIdValido_QuandoObterPorId_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var id = _faker.Random.Long(1);
            var codafDto = new CodafListaPresencaDto
            {
                Id = id,
                PropostaId = _faker.Random.Long(1),
                PropostaTurmaId = _faker.Random.Long(1)
            };
            _mockCasoDeUsoObterPorId
                .Setup(x => x.ExecutarAsync(id))
                .ReturnsAsync(Resultado<CodafListaPresencaDto>.DeSucesso(codafDto));
            // Act
            var resultado = await _controller.ObterPorId(id) as ObjectResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultadoValor = resultado.Value as CodafListaPresencaDto;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(codafDto);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterListaPaginada_EntaoDeveChamarCasoDeUsoListar()
        {
            // Arrange
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            _mockCasoDeUsoListar
                .Setup(x => x.ExecutarAsync(filtroDto))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>.DeSucesso(
                    new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>([], 0, 0)));
            // Act
            await _controller.ObterListaPaginada(filtroDto);
            // Assert
            _mockCasoDeUsoListar.Verify(x => x.ExecutarAsync(filtroDto), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoObterListaPaginada_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var filtroDto = new FiltroListaPresencaCodafDto
            {
                NomeFormacao = _faker.Lorem.Word(),
                CodigoFormacao = _faker.Random.Int(1),
                NumeroPagina = 1,
                NumeroRegistros = 10
            };
            var paginacaoResultadoDto = new PaginacaoResultadoDto<ListaPresencaCodafResumoDto>([], 0, 0);
            _mockCasoDeUsoListar
                .Setup(x => x.ExecutarAsync(filtroDto))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<ListaPresencaCodafResumoDto>>.DeSucesso(paginacaoResultadoDto));
            // Act
            var resultado = await _controller.ObterListaPaginada(filtroDto) as ObjectResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultadoValor = resultado.Value as PaginacaoResultadoDto<ListaPresencaCodafResumoDto>;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(paginacaoResultadoDto);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarObterInscritosPorTurma_EntaoDeveChamarCasoDeUsoListarInscritosTurma()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var numeroPagina = 1;
            var numeroRegistros = 10;
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(
                    new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0)));
            // Act
            await _controller.ObterInscritosPorTurma(propostaTurmaId, numeroPagina, numeroRegistros);
            // Assert
            _mockCasoDeUsoListarInscritosTurma.Verify(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros), Times.Once);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarObterInscritosPorTurma_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var numeroPagina = 1;
            var numeroRegistros = 10;
            var paginacaoResultadoDto = new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0);
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, numeroPagina, numeroRegistros))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(paginacaoResultadoDto));
            // Act
            var resultado = await _controller.ObterInscritosPorTurma(propostaTurmaId, numeroPagina, numeroRegistros) as ObjectResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultadoValor = resultado.Value as PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(paginacaoResultadoDto);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarTurmaPossuiListaPresenca_EntaoDeveRetornarResultadoSucesso()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            var listaPresencaId = _faker.Random.Long(1);
            _mockCasoDeUsoListarInscritosTurma
                .Setup(x => x.ExecutarAsync(propostaTurmaId, 1, 10))
                .ReturnsAsync(Resultado<PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>>.DeSucesso(
                    new PaginacaoResultadoDto<CodafInscritoTurmaListaPresencaRetornoDto>([], 0, 0)));
            // Act
            await _controller.ObterInscritosPorTurma(propostaTurmaId);
            // Assert
            _mockCasoDeUsoListarInscritosTurma.Verify(x => x.ExecutarAsync(propostaTurmaId, 1, 10), Times.Once);
        }

        [Fact]
        public async Task DadoUmaPropostaTurmaId_QuandoChamarTurmaPossuiListaPresenca_EntaoDeveChamarCasoDeUsoTurmaPossuiCodafListaPresenca()
        {
            // Arrange
            var propostaTurmaId = _faker.Random.Long(1);
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca
                .Setup(x => x.ExecutarAsync(propostaTurmaId))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.TurmaPossuiListaPresenca(propostaTurmaId);
            // Assert
            _mockCasoDeUsoTurmaPossuiCodafListaPresenca.Verify(x => x.ExecutarAsync(propostaTurmaId), Times.Once);
        }

        [Fact]
        public async Task DadoUmaRetificacaoId_QuandoRemoverRetificacao_EnaoDeveChamarCasoDeUsoRemoverCodafRetificacaoListaPresenca()
        {
            // Arrange
            var retificacaoId = _faker.Random.Long(1);
            _mockCasoDeUsoRemoverRetificacao
                .Setup(x => x.ExecutarAsync(retificacaoId))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            await _controller.RemoverRetificacao(retificacaoId);
            // Assert
            _mockCasoDeUsoRemoverRetificacao.Verify(x => x.ExecutarAsync(retificacaoId), Times.Once);
        }

        [Fact]
        public async Task DadoSolicitacaoDeModelo_QuandoArquivoExistir_EntaoDeveRetornarFileStreamResult()
        {
            // Arrange
            var nomeArquivo = "TermoResponsabilidadeModelo.pdf";
            var contentType = "application/pdf";
            var memoryStream = new MemoryStream([1, 2, 3]);

            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, memoryStream);
            var resultadoSucesso = Resultado<ArquivoDto>.DeSucesso(arquivoDto);

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Setup(x => x.Executar())
                .Returns(resultadoSucesso);

            // Act
            var resultado = await _controller.ObterModeloTermoResponsabilidade();

            // Assert
            var fileResult = resultado.Should().BeOfType<FileStreamResult>().Subject;

            fileResult.ContentType.Should().Be(contentType);
            fileResult.FileDownloadName.Should().Be(nomeArquivo);
            fileResult.FileStream.Should().BeSameAs(memoryStream);

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Verify(x => x.Executar(), Times.Once);
        }

        [Fact]
        public async Task DadoSolicitacaoDeModelo_QuandoArquivoNaoForEncontrado_EntaoDeveRetornarNotFound()
        {
            // Arrange
            var erro = Erro.NaoEncontrado("Modelo não encontrado.");

            _mockCasoDeUsoObterModeloTermoResponsabilidadeCodaf
                .Setup(x => x.Executar())
                .Returns(erro);

            // Act
            var resultado = await _controller.ObterModeloTermoResponsabilidade();

            // Assert
            var notFoundResult = resultado.Should().BeOfType<NotFoundObjectResult>().Subject;

            notFoundResult.StatusCode.Should().Be(404);

            var valorRetorno = notFoundResult.Value;
            valorRetorno.Should().NotBeNull();
        }

        [Fact]
        public async Task DadoUmArquivoValido_QuandoChamarUploadAnexoTemporario_EntaoDeveChamarCasoDeUsoUploadAnexoTemporarioCodafListaPresenca()
        {
            // Arrange
            var arquivoMock = new Mock<IFormFile>();
            arquivoMock.Setup(a => a.Length).Returns(1024); // 1 KB
            arquivoMock.Setup(a => a.FileName).Returns("documento.pdf");
            arquivoMock.Setup(a => a.ContentType).Returns("application/pdf");
            arquivoMock.Setup(a => a.OpenReadStream()).Returns(new MemoryStream([1, 2, 3]));
            var arquivoDto = arquivoMock.Object;
            var arquivoTemporarioDto = new CodafAnexoTemporarioDto { ArquivoCodigo = Guid.NewGuid(), NomeArquivo = "documento.pdf", ContentType = "application/pdf", TamanhoBytes = 1024 };
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca
                .Setup(x => x.ExecutarAsync(arquivoDto))
                .ReturnsAsync(Resultado<CodafAnexoTemporarioDto>.DeSucesso(arquivoTemporarioDto));
            // Act
            await _controller.UploadAnexoTemporario(arquivoDto);
            // Assert
            _mockCasoDeUsoUploadAnexoTemporarioCodafListaPresenca.Verify(x => x.ExecutarAsync(arquivoDto), Times.Once);
        }

        [Fact]
        public async Task DadoUmIdCodafListaPresenca_QuandoChamarEnviarParaDf_EntaoDeveChamarCasoDeUsoEnviarParaDfCodafListaPresenca()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1);
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.EnviarParaDf(codafListaPresencaId);
            // Assert
            _mockCasoDeUsoEnviarParaDfCodafListaPresenca.Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }

        [Fact]
        public async Task DadoUmIdCodafListaPresencaEJustificativa_QuandoChamarDevolverParaCorrecao_EntaoDeveChamarCasoDeUsoDevolverParaCorrecaoCodafListaPresenca()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1);
            var justificativa = _faker.Lorem.Sentence();
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca
                .Setup(x => x.ExecutarAsync(codafListaPresencaId, justificativa))
                .ReturnsAsync(Resultado<bool>.DeSucesso(true));
            // Act
            await _controller.DevolverParaCorrecao(codafListaPresencaId, justificativa);
            // Assert
            _mockCasoDeUsoDevolverParaCorrecaoCodafListaPresenca.Verify(x => x.ExecutarAsync(codafListaPresencaId, justificativa), Times.Once);
        }
    }
}