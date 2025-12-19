using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
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
        private readonly CodafListaPresencaController _controller;
        private readonly Faker _faker;

        public CodafListaPresencaControllerTests()
        {
            var mocker = new AutoMocker();
            _mockCasoDeUsoCriar = mocker.GetMock<ICasoDeUsoCriarCodafListaPresenca>();
            _mockCasoDeUsoAtualizar = mocker.GetMock<ICasoDeUsoAtualizarCodafListaPresenca>();
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
    }
}