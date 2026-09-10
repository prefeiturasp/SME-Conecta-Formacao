using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterAreaPromotoraPorIdTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterAreaPromotoraPorId _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterAreaPromotoraPorIdTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterAreaPromotoraPorId>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoIdValido_QuandoExecutar_EntaoDeveRetornarAreaPromotoraComVisaoId()
        {
            // Arrange
            var idAreaPromotora = _faker.Random.Long(1, 100);
            var areaPromotoraCompletaDto = new AreaPromotoraCompletoDTO { GrupoId = _faker.Random.Guid() };
            var grupoDto = new GrupoDTO { VisaoId = _faker.Random.Int(1, 10) };

            _mediatorMock.Setup(m => m.Send(It.Is<ObterAreaPromotoraCompletaPorIdQuery>(q => q.Id == idAreaPromotora), default))
                .ReturnsAsync(areaPromotoraCompletaDto);

            _mediatorMock.Setup(m => m.Send(It.Is<ObterGrupoPorIdQuery>(q => q.GrupoId == areaPromotoraCompletaDto.GrupoId), default))
                .ReturnsAsync(grupoDto);

            // Act
            var resultado = await _sut.Executar(idAreaPromotora);

            // Assert
            resultado.Should().NotBeNull();
            resultado.VisaoId.Should().Be(grupoDto.VisaoId);
            resultado.Should().BeSameAs(areaPromotoraCompletaDto);

            _mediatorMock.Verify(m => m.Send(It.Is<ObterAreaPromotoraCompletaPorIdQuery>(q => q.Id == idAreaPromotora), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterGrupoPorIdQuery>(q => q.GrupoId == areaPromotoraCompletaDto.GrupoId), default), Times.Once);
        }
    }
}
