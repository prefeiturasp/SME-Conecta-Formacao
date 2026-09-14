using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Formacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Formacao
{
    public class CasoDeUsoObterFormacaoDetalhadaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterFormacaoDetalhada _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterFormacaoDetalhadaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterFormacaoDetalhada>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoExecutar_EntaoDeveRetornarDetalhesDaFormacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, long.MaxValue);
            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = _faker.Lorem.Sentence(),
                AreaPromotora = _faker.Company.CompanyName()
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterFormacaoDetalhadaPorIdQuery>(q => q.Id == propostaId && q.Filtro == null),
                    default))
                .ReturnsAsync(formacaoDetalhadaDto);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().BeEquivalentTo(formacaoDetalhadaDto);
            
            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterFormacaoDetalhadaPorIdQuery>(q => q.Id == propostaId && q.Filtro == null),
                default), Times.Once);
        }
    }
}
