using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoObterFormatosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterFormatos _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterFormatosTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoObterFormatos>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoTipoFormacao_QuandoExecutar_EntaoDeveRetornarListaDeFormatos()
        {
            // Arrange
            var tipoFormacao = _faker.PickRandom<TipoFormacao>();
            var formatos = new List<RetornoListagemDTO>
            {
                new RetornoListagemDTO { Id = 1, Descricao = "Formato 1" },
                new RetornoListagemDTO { Id = 2, Descricao = "Formato 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterFormatosQuery>(q => q.TipoFormacao == tipoFormacao), default))
                .ReturnsAsync(formatos);

            // Act
            var resultado = await _sut.Executar(tipoFormacao);

            // Assert
            resultado.Should().BeEquivalentTo(formatos);

            _mediatorMock.Verify(m => m.Send(
                It.Is<ObterFormatosQuery>(q => q.TipoFormacao == tipoFormacao), default), Times.Once);
        }
    }
}
