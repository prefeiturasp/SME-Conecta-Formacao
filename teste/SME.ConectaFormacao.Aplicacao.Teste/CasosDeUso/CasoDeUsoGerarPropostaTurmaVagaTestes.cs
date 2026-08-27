using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarPropostaTurmaVagaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoGerarPropostaTurmaVaga _sut;
        private readonly Faker _faker;

        public CasoDeUsoGerarPropostaTurmaVagaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoGerarPropostaTurmaVaga>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoMensagemRabbitValida_QuandoChamarExecutar_EntaoDeveGerarPropostaTurmaVagaComSucesso()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var qtdVagasTurma = _faker.Random.Int(10, 50);
            var param = new MensagemRabbit(propostaId.ToString());
            
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { QuantidadeVagasTurma = qtdVagasTurma };

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), default)).ReturnsAsync(proposta);
            _mediatorMock.Setup(m => m.Send(It.Is<GerarPropostaTurmaVagaCommand>(c => c.PropostaId == propostaId && c.QuantidadeVagasTurma == qtdVagasTurma), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarPropostaTurmaVagaCommand>(), default), Times.Once);
        }
    }
}
