using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using EntidadeProposta = SME.ConectaFormacao.Dominio.Entidades.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Propostas
{
    public class CasoDeUsoRecusarPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRecusarProposta _sut;
        private readonly Faker _faker;

        public CasoDeUsoRecusarPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRecusarProposta>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoExecutar_EntaoDeveLancarNegocioExceptionPropostaNaoEncontrada()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var dto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntidadeProposta?)null);

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaId, dto);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaExcluida_QuandoExecutar_EntaoDeveLancarNegocioExceptionPropostaNaoEncontrada()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var dto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };
            var proposta = new EntidadeProposta
            {
                Id = propostaId,
                Situacao = SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                Excluido = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaId, dto);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaComSituacaoInvalida_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var dto = new PropostaJustificativaDTO { Justificativa = _faker.Lorem.Sentence() };
            var proposta = new EntidadeProposta
            {
                Id = propostaId,
                Situacao = SituacaoProposta.Publicada,
                Excluido = false
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaId, dto);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECER_DF);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task DadoJustificativaNaoInformada_QuandoExecutar_EntaoDeveLancarNegocioException(string? justificativa)
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var dto = new PropostaJustificativaDTO { Justificativa = justificativa! };
            var proposta = new EntidadeProposta
            {
                Id = propostaId,
                Situacao = SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                Excluido = false
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Act
            Func<Task> act = async () => await _sut.Executar(propostaId, dto);

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaValidaEPerfilNaoAdminDF_QuandoExecutar_EntaoDeveEnviarCommandsNaoPublicarRabbitERetornarTrue()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativa = _faker.Lorem.Sentence();
            var dto = new PropostaJustificativaDTO { Justificativa = justificativa };
            var proposta = new EntidadeProposta
            {
                Id = propostaId,
                Situacao = SituacaoProposta.AguardandoValidacaoFinalPelaDF,
                Excluido = false
            };
            var perfilNaoAdminDF = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(perfilNaoAdminDF);

            // Act
            var resultado = await _sut.Executar(propostaId, dto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarPropostaCommand>(c => c.PropostaId == propostaId && c.Situacao == SituacaoProposta.Recusada), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarPropostaMovimentacaoCommand>(c => c.PropostaId == propostaId && c.Situacao == SituacaoProposta.Recusada && c.Justificativa == justificativa), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoPropostaValidaEPerfilAdminDF_QuandoExecutar_EntaoDeveEnviarCommandsPublicarRabbitERetornarTrue()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1);
            var justificativa = _faker.Lorem.Sentence();
            var dto = new PropostaJustificativaDTO { Justificativa = justificativa };
            var proposta = new EntidadeProposta
            {
                Id = propostaId,
                Situacao = SituacaoProposta.AguardandoAnaliseParecerPelaDF,
                Excluido = false
            };
            var perfilAdminDF = Perfis.ADMIN_DF;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarPropostaMovimentacaoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(perfilAdminDF);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId, dto);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarPropostaCommand>(c => c.PropostaId == propostaId && c.Situacao == SituacaoProposta.Recusada), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarPropostaMovimentacaoCommand>(c => c.PropostaId == propostaId && c.Situacao == SituacaoProposta.Recusada && c.Justificativa == justificativa), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.NotificarAreaPromotoraSobreValidacaoFinalPelaDF && (long)c.Filtros == propostaId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
