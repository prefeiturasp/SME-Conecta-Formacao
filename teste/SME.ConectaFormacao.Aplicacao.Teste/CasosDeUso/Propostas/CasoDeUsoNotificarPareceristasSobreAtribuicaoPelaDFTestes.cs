using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDFTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF _casoDeUso;

        public CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDFTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoNotificarPareceristasSobreAtribuicaoPelaDF>();
        }

        [Fact]
        public async Task DadoPropostaIdZero_QuandoExecutar_EntaoDeveLancarExcecaoParametroInvalido()
        {
            // Arrange
            var dto = new NotificacaoPropostaPareceristasDTO(0, Enumerable.Empty<PropostaPareceristaResumidoDTO>());
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
            var dto = new NotificacaoPropostaPareceristasDTO(1, Enumerable.Empty<PropostaPareceristaResumidoDTO>());
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync((SME.ConectaFormacao.Dominio.Entidades.Proposta)null!);

            // Act
            var acao = async () => await _casoDeUso.Executar(param);

            // Assert
            var excecao = await acao.Should().ThrowAsync<Exception>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoSituacaoValida_QuandoExecutar_EntaoDeveGerarNotificacaoERetornarResultado()
        {
            // Arrange
            var pareceristas = new List<PropostaPareceristaResumidoDTO> { new PropostaPareceristaResumidoDTO("login-1", "Nome 1") };
            var dto = new NotificacaoPropostaPareceristasDTO(1, pareceristas);
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync(proposta);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GerarNotificacaoPareceristaCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<GerarNotificacaoPareceristaCommand>(c => c.Proposta == proposta && c.Pareceristas.Count() == dto.Pareceristas.Count()), default), Times.Once);
        }

        [Fact]
        public async Task DadoSituacaoInvalida_QuandoExecutar_EntaoDeveRetornarFalsoENaoGerarNotificacao()
        {
            // Arrange
            var dto = new NotificacaoPropostaPareceristasDTO(1, Enumerable.Empty<PropostaPareceristaResumidoDTO>());
            var param = new MensagemRabbit(JsonSerializer.Serialize(dto));
            var proposta = new SME.ConectaFormacao.Dominio.Entidades.Proposta { Situacao = SituacaoProposta.Rascunho };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == dto.PropostaId), default))
                .ReturnsAsync(proposta);

            // Act
            var resultado = await _casoDeUso.Executar(param);

            // Assert
            resultado.Should().BeFalse();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GerarNotificacaoPareceristaCommand>(), default), Times.Never);
        }
    }
}
