using System;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDFTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF _sut;
        private readonly Faker _faker;

        public CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDFTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoNotificarAreaPromotoraSobreValidacaoFinalPelaDF>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaAprovada_QuandoChamarExecutar_EntaoDeveGerarNotificacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var param = new MensagemRabbit(propostaId.ToString());
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { Situacao = SituacaoProposta.Aprovada };

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), default)).ReturnsAsync(proposta);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>(), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaRecusada_QuandoChamarExecutar_EntaoDeveGerarNotificacao()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var param = new MensagemRabbit(propostaId.ToString());
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { Situacao = SituacaoProposta.Recusada };

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), default)).ReturnsAsync(proposta);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>(), default)).ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaComSituacaoDiferenteDeAprovadaOuRecusada_QuandoChamarExecutar_EntaoNaoDeveGerarNotificacaoERetornarFalse()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var param = new MensagemRabbit(propostaId.ToString());
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { Situacao = SituacaoProposta.Cadastrada };

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), default)).ReturnsAsync(proposta);

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoAreaPromotoraSobreValidacaoFinalCommand>(), default), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaIdZero_QuandoChamarExecutar_EntaoDeveLancarException()
        {
            // Arrange
            var param = new MensagemRabbit("0");

            // Act
            var act = async () => await _sut.Executar(param);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(MensagemNegocio.PARAMETRO_INVALIDO);
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoChamarExecutar_EntaoDeveLancarException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var param = new MensagemRabbit(propostaId.ToString());

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), default)).ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta)null);

            // Act
            var act = async () => await _sut.Executar(param);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }
    }
}
