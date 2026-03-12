using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ues;
using SME.ConectaFormacao.Infra.Dados.Dtos.Ues;
using SME.ConectaFormacao.Webapi.Controllers;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class UeControllerTests
    {
        private readonly Mock<ICasoDeUsoObterAutocompletarNomeUe> _casoDeUsoAutocompletarNomeMock;
        private readonly UeController _sut;
        private readonly Faker _faker;

        public UeControllerTests()
        {
            var mocker = new AutoMocker();

            _casoDeUsoAutocompletarNomeMock = mocker.GetMock<ICasoDeUsoObterAutocompletarNomeUe>();
            _sut = mocker.CreateInstance<UeController>();
            _faker = new();
        }

        [Fact]
        public async Task DadoUmTermoDeBuscaQualquer_QuandoChmarAutocompletarNome_DeveRetornarResultadoDeSucesso()
        {
            // Arrange
            var filtro = new FiltroAutocompletarNomeUeDto
            {
                TermoBusca = _faker.Random.Word(),
                DreId = 1,
                NumeroPagina = 1,
                NumeroRegistros = 10
            };

            var resultadoEsperado = new PaginacaoResultadoDto<AutocompletarNomeUeDto>(
            [
                new() { Id = Guid.NewGuid(), Nome = _faker.Company.CompanyName() },
                new() { Id = Guid.NewGuid(), Nome = _faker.Company.CompanyName() }
            ], 0, 10);

            _casoDeUsoAutocompletarNomeMock
                .Setup(c => c.ExecutarAsync(filtro))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _sut.AutocompletarNomeAsync(filtro);

            // Assert
            Assert.IsType<OkObjectResult>(resultado);
        }
    }
}
