using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEnviarPropostaPareceristaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEnviarPropostaParecerista _sut;
        private readonly Faker _faker;

        public CasoDeUsoEnviarPropostaPareceristaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoEnviarPropostaParecerista>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaValida_QuandoChamarExecutar_EntaoDeveEnviarParecerComSucesso()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var usuario = new Usuario { Login = _faker.Internet.UserName() };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default)).ReturnsAsync(usuario);
            _mediatorMock.Setup(m => m.Send(It.IsAny<EnviarParecerPareceristaCommand>(), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarParecerPareceristaCommand>(c =>
                c.PropostaId == propostaId &&
                c.Situacao == SituacaoParecerista.Enviada &&
                c.Justificativa == string.Empty), default), Times.Once);
        }
    }
}
