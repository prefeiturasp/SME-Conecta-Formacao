using Bogus;
using FluentAssertions;
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
        private readonly Mock<ICasoDeUsoRemoverCodafRetificacaoListaPresenca> _mockCasoDeUsoRemoverRetificacao;
        private readonly Mock<ICasoDeUsoExcluirCodafListaPresenca> _mockCasoDeUsoExcluirCodafListaPresenca;
        private readonly Mock<ICasoDeUsoGerarRelatorioCodaf> _mockCasoDeUsoGerarRelatorioCodaf;
        private readonly Mock<ICasoDeUsoSalvarInscritosCodaf> _mockCasoDeUsoSalvarInscritosCodaf;
        private readonly CodafListaPresencaController _controller;
        private readonly Faker _faker;

        public CodafListaPresencaControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoCriar = mocker.GetMock<ICasoDeUsoCriarCodafListaPresenca>();
            _mockCasoDeUsoAtualizar = mocker.GetMock<ICasoDeUsoAtualizarCodafListaPresenca>();
            _mockCasoDeUsoListar = mocker.GetMock<ICasoDeUsoListarCodafListaPresenca>();
            _mockCasoDeUsoObterPorId = mocker.GetMock<ICasoDeUsoObterCodafListaPresencaPorId>();
            _mockCasoDeUsoRemoverRetificacao = mocker.GetMock<ICasoDeUsoRemoverCodafRetificacaoListaPresenca>();
            _mockCasoDeUsoExcluirCodafListaPresenca = mocker.GetMock<ICasoDeUsoExcluirCodafListaPresenca>();
            _mockCasoDeUsoGerarRelatorioCodaf = mocker.GetMock<ICasoDeUsoGerarRelatorioCodaf>();
            _mockCasoDeUsoSalvarInscritosCodaf = mocker.GetMock<ICasoDeUsoSalvarInscritosCodaf>();
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
            await _controller.Cadastrar(cadastroDto, _mockCasoDeUsoCriar.Object);

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
            var resultado = await _controller.Cadastrar(cadastroDto, _mockCasoDeUsoCriar.Object) as ObjectResult;

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
            await _controller.Atualizar(id, edicaoDto, _mockCasoDeUsoAtualizar.Object);

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
            var resultado = await _controller.Atualizar(id, edicaoDto, _mockCasoDeUsoAtualizar.Object) as StatusCodeResult;
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
            await _controller.ObterPorId(id, _mockCasoDeUsoObterPorId.Object);
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
            var resultado = await _controller.ObterPorId(id, _mockCasoDeUsoObterPorId.Object) as ObjectResult;
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
            await _controller.ObterListaPaginada(filtroDto, _mockCasoDeUsoListar.Object);
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
            var resultado = await _controller.ObterListaPaginada(filtroDto, _mockCasoDeUsoListar.Object) as ObjectResult;
            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var resultadoValor = resultado.Value as PaginacaoResultadoDto<ListaPresencaCodafResumoDto>;
            resultadoValor.Should().NotBeNull();
            resultadoValor.Should().BeEquivalentTo(paginacaoResultadoDto);
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
            await _controller.RemoverRetificacao(retificacaoId, _mockCasoDeUsoRemoverRetificacao.Object);
            // Assert
            _mockCasoDeUsoRemoverRetificacao.Verify(x => x.ExecutarAsync(retificacaoId), Times.Once);
        }

        [Fact]
        public async Task DadoUmIdCodafListaPresenca_QuandoChamarExcluir_EntaoDeveChamarCasoDeUso()
        {
            // Arrange
            var codafListaPresencaId = _faker.Random.Long(1);
            _mockCasoDeUsoExcluirCodafListaPresenca
                .Setup(x => x.ExecutarAsync(codafListaPresencaId))
                .ReturnsAsync(Resultado.DeSucesso());
            // Act
            await _controller.Excluir(codafListaPresencaId, _mockCasoDeUsoExcluirCodafListaPresenca.Object);
            // Assert
            _mockCasoDeUsoExcluirCodafListaPresenca.Verify(x => x.ExecutarAsync(codafListaPresencaId), Times.Once);
        }

        [Fact]
        public async Task DadoImprimirRelatorioCodaf_QuandoRetornarSucesso__EntaoDeveRetornarFileStreamResult()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            var nomeArquivo = $"CODAF_{_faker.Random.Int(10000, 99999)}-Turma {_faker.Random.Word()}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var memoryStream = new MemoryStream([1, 2, 3]);

            var arquivoDto = new ArquivoDto(nomeArquivo, contentType, memoryStream);
            var resultadoSucesso = Resultado<ArquivoDto>.DeSucesso(arquivoDto);

            _mockCasoDeUsoGerarRelatorioCodaf
                .Setup(x => x.ExecutarAsync(codafId))
                .ReturnsAsync(resultadoSucesso);

            // Act
            var resultado = await _controller.ImprimirRelatorioCodafAsync(codafId, _mockCasoDeUsoGerarRelatorioCodaf.Object) as FileStreamResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.FileStream.Should().BeSameAs(memoryStream);
            resultado.ContentType.Should().Be(contentType);
            resultado.FileDownloadName.Should().Be(nomeArquivo);
        }

        [Fact]
        public async Task DadoImprimirRelatorioCodaf_QuandoRetornarErroNaoEncontrado__EntaoDeveRetornarNotFound()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            var erro = Erro.NaoEncontrado("Lista de presença CODAF não encontrada.");
            _mockCasoDeUsoGerarRelatorioCodaf
                .Setup(x => x.ExecutarAsync(codafId))
                .ReturnsAsync(erro);
            // Act
            var resultado = await _controller.ImprimirRelatorioCodafAsync(codafId, _mockCasoDeUsoGerarRelatorioCodaf.Object) as UnprocessableEntityObjectResult;
            // Assert
            resultado.Should().NotBeNull();

            // *Alterado para o 422 pois está sendo omitido o corpo da resposta com o 404, e o front não consegue ler a mensagem de erro (by Diego Moreno - 2026-08-21)
            resultado.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task DadoUmaListaDeInscritos_QuandoSalvarInscritos_EntaoDeveRetornar204()
        {
            // Arrange
            var codafId = _faker.Random.Long(1);
            _mockCasoDeUsoSalvarInscritosCodaf
                .Setup(x => x.ExecutarAsync(It.IsAny<List<CodafInscritoListaPresencaSalvarDto>>(), It.IsAny<long>()))
                .ReturnsAsync(Resultado.DeSucesso());

            // Act
            var resultado = await _controller.SalvarInscritosAsync(codafId, [], _mockCasoDeUsoSalvarInscritosCodaf.Object) as StatusCodeResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }
    }
}