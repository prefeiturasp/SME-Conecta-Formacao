using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAprovarPropostaPareceristaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoAprovarPropostaParecerista _sut;
        private readonly Faker _faker;

        public CasoDeUsoAprovarPropostaPareceristaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoAprovarPropostaParecerista>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaValida_QuandoChamarExecutar_EntaoDeveAprovarComSucesso()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativaDto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };
            var usuario = new Usuario { Login = _faker.Internet.UserName() };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), default)).ReturnsAsync(usuario);
            _mediatorMock.Setup(m => m.Send(It.IsAny<EnviarParecerPareceristaCommand>(), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId, justificativaDto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarParecerPareceristaCommand>(c =>
                c.PropostaId == propostaId &&
                c.Situacao == SituacaoParecerista.Aprovada &&
                c.Justificativa == justificativaDto.Justificativa), default), Times.Once);
        }
    }
}
