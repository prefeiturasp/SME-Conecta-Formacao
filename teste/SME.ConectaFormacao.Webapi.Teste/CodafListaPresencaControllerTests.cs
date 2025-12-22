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
        private readonly CodafListaPresencaController _controller;
        private readonly Faker _faker;

        public CodafListaPresencaControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoCriar = mocker.GetMock<ICasoDeUsoCriarCodafListaPresenca>();
            _mockCasoDeUsoAtualizar = mocker.GetMock<ICasoDeUsoAtualizarCodafListaPresenca>();
            _mockCasoDeUsoListar = mocker.GetMock<ICasoDeUsoListarCodafListaPresenca>();
            _mockCasoDeUsoObterPorId = mocker.GetMock<ICasoDeUsoObterCodafListaPresencaPorId>();
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
    }
}