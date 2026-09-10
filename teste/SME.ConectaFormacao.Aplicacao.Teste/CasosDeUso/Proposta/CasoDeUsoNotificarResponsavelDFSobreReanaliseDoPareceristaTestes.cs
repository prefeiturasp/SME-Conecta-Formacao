using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;
using Xunit;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Proposta
{
    public class CasoDeUsoNotificarResponsavelDFSobreReanaliseDoPareceristaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista _casoDeUso;

        public CasoDeUsoNotificarResponsavelDFSobreReanaliseDoPareceristaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoNotificarResponsavelDFSobreReanaliseDoParecerista>();
        }

        [Fact]
        public async Task DadoPropostaIdZero_QuandoExecutar_EntaoDeveLancarExcecaoParametroInvalido()
        {
            // Arrange
            var dto = new NotificacaoPropostaPareceristaDTO(0, null!);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var acao = async () => await _casoDeUso.Executar(param);

            // Assert
            var excecao = await acao.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PARAMETRO_INVALIDO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), default), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaNaoEncontrada_QuandoExecutar_EntaoDeveLancarExcecaoPropostaNaoEncontrada()
        {
            // Arrange
            var dto = new NotificacaoPropostaPareceristaDTO(1, null!);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync((EntidadeProposta)null!);

            // Act
            var acao = async () => await _casoDeUso.Executar(param);

            // Assert
            var excecao = await acao.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoResponsavelDFCommand>(), default), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task DadoPropostaSemRfResponsavelDf_QuandoExecutar_EntaoDeveLancarExcecaoResponsavelDfNaoEncontrado(string? rfResponsavelDf)
        {
            // Arrange
            var dto = new NotificacaoPropostaPareceristaDTO(1, null!);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));
            var proposta = new EntidadeProposta
            {
                Id = dto.PropostaId,
                RfResponsavelDf = rfResponsavelDf!
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync(proposta);

            // Act
            var acao = async () => await _casoDeUso.Executar(param);

            // Assert
            var excecao = await acao.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.RESPONSAVEL_DF_NAO_ENCONTRADO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoResponsavelDFCommand>(), default), Times.Never);
        }

        [Theory]
        [InlineData(SituacaoProposta.AguardandoReanalisePeloParecerista)]
        [InlineData(SituacaoProposta.AguardandoValidacaoFinalPelaDF)]
        public async Task DadoSituacaoQuePermiteNotificacao_QuandoExecutar_EntaoDeveGerarNotificacaoERetornarResultado(SituacaoProposta situacao)
        {
            // Arrange
            var parecerista = new PropostaPareceristaResumidoDTO("login-teste", "Nome Teste");
            var dto = new NotificacaoPropostaPareceristaDTO(1, parecerista);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));
            var proposta = new EntidadeProposta
            {
                Id = dto.PropostaId,
                RfResponsavelDf = "1234567",
                Situacao = situacao
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync(proposta);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarNotificacaoResponsavelDFCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(
                It.Is<GerarNotificacaoResponsavelDFCommand>(c =>
                    c.Proposta == proposta &&
                    c.Parecerista.Login == dto.Parecerista.Login),
                default), Times.Once);
        }

        [Fact]
        public async Task DadoSituacaoQueNaoAtendeCondicoes_QuandoExecutar_EntaoDeveRetornarFalsoENaoGerarNotificacao()
        {
            // Arrange
            var parecerista = new PropostaPareceristaResumidoDTO("login-teste", "Nome Teste");
            var dto = new NotificacaoPropostaPareceristaDTO(1, parecerista);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));
            var proposta = new EntidadeProposta
            {
                Id = dto.PropostaId,
                RfResponsavelDf = "1234567",
                Situacao = SituacaoProposta.Rascunho
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _casoDeUso.Executar(param);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoResponsavelDFCommand>(), default), Times.Never);
        }
    }
}
