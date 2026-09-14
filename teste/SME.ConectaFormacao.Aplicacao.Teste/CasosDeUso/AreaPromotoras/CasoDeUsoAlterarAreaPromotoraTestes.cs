using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.AreaPromotoras
{
    public class CasoDeUsoAlterarAreaPromotoraTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly CasoDeUsoAlterarAreaPromotora _casoDeUso;

        public CasoDeUsoAlterarAreaPromotoraTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoAlterarAreaPromotora>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoComandoValido_QuandoExecutar_EntaoDeveEnviarComandoERetornarResultado()
        {
            // Arrange
            var id = _faker.Random.Long(1);
            var dto = new AreaPromotoraDTO { Nome = _faker.Random.String2(10), GrupoId = _faker.Random.Guid() };
            var resultadoEsperado = _faker.Random.Bool();

            _mediatorMock
                .Setup(m => m.Send(It.Is<AlterarAreaPromotoraCommand>(c => c.Id == id && c.AreaPromotoraDTO == dto), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(id, dto);

            // Assert
            resultado.Should().Be(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<AlterarAreaPromotoraCommand>(c => c.Id == id && c.AreaPromotoraDTO == dto), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
