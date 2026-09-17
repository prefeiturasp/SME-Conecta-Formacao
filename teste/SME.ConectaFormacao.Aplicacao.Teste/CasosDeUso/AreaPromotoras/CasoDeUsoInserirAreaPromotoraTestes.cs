using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.AreaPromotoras
{
    public class CasoDeUsoInserirAreaPromotoraTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Faker _faker;
        private readonly CasoDeUsoInserirAreaPromotora _casoDeUso;

        public CasoDeUsoInserirAreaPromotoraTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoInserirAreaPromotora>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoDtoValido_QuandoExecutar_EntaoDeveEnviarComandoERetornarId()
        {
            // Arrange
            var dto = new AreaPromotoraDTO { Nome = _faker.Random.String2(10), GrupoId = _faker.Random.Guid() };
            var idRetornado = _faker.Random.Long(1);

            _mediatorMock
                .Setup(m => m.Send(It.Is<InserirAreaPromotoraCommand>(c => c.AreaPromotoraDTO == dto), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idRetornado);

            // Act
            var resultado = await _casoDeUso.Executar(dto);

            // Assert
            resultado.Should().Be(idRetornado);
            _mediatorMock.Verify(m => m.Send(It.Is<InserirAreaPromotoraCommand>(c => c.AreaPromotoraDTO == dto), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
