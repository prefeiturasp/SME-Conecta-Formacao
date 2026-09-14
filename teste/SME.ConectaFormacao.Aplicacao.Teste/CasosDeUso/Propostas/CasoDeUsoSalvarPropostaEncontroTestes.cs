using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaEncontro;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoSalvarPropostaEncontroTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarPropostaEncontro _sut;
        private readonly Faker _faker;

        public CasoDeUsoSalvarPropostaEncontroTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoSalvarPropostaEncontro>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoParametrosValidos_QuandoExecutar_EntaoDeveEnviarCommandERetornarIdGerado()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var dto = new PropostaEncontroDto { Id = _faker.Random.Long(1, 100) };
            var idEsperado = _faker.Random.Long(1, 100);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarPropostaEncontroCommand>(c => c.PropostaId == id && c.EncontroDto == dto), default))
                .ReturnsAsync(idEsperado);

            // Act
            var resultado = await _sut.Executar(id, dto);

            // Assert
            resultado.Should().Be(idEsperado);
            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarPropostaEncontroCommand>(c => c.PropostaId == id && c.EncontroDto == dto),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoErroNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var id = _faker.Random.Long(1, 100);
            var dto = new PropostaEncontroDto();
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaEncontroCommand>(), default))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(id, dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
        }
    }
}
