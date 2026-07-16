using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Comandos.Propostas
{
    public class EnviarParecerPareceristaCommandHandlerTestes
    {
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly EnviarParecerPareceristaCommandHandler _sut;
        private readonly Faker _faker;

        public EnviarParecerPareceristaCommandHandlerTestes()
        {
            var mocker = new AutoMocker();

            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();

            _sut = mocker.CreateInstance<EnviarParecerPareceristaCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoPropostaNula_QuandoHandle_EntaoLancaNegocioException_PropostaNaoEncontrada()
        {
            // Arrange
            var comando = GerarComandoValido();

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((Proposta)null!);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaExcluida_QuandoHandle_EntaoLancaNegocioException_PropostaNaoEncontrada()
        {
            // Arrange
            var comando = GerarComandoValido();
            var proposta = new Proposta { Excluido = true };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(proposta);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaComSituacaoInvalida_QuandoHandle_EntaoLancaNegocioException_SituacaoInvalida()
        {
            // Arrange
            var comando = GerarComandoValido();
            var proposta = new Proposta { Situacao = SituacaoProposta.Cadastrada };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(proposta);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_AGUARDANDO_PARECERISTA);
        }

        [Fact]
        public async Task DadoPareceristaNaoEncontrado_QuandoHandle_EntaoLancaNegocioException_NaoEhParecerista()
        {
            // Arrange
            var comando = GerarComandoValido();
            var proposta = new Proposta { Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPorIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(proposta);

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((PropostaParecerista)null!);

            // Act
            var acao = async () => await _sut.Handle(comando, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.USUARIO_LOGADO_NAO_E_PARECERISTA_DA_PROPOSTA);
        }

        [Fact]
        public async Task DadoPropostaAguardandoAnaliseComOutrosPareceristasPendentes_QuandoHandle_EntaoApenasNotificaViaRabbitMq()
        {
            // Arrange
            var comando = GerarComandoValido();
            var proposta = new Proposta { Id = comando.PropostaId, Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista };
            var pareceristaAtual = new PropostaParecerista { Id = 1, RegistroFuncional = comando.RegistroFuncional };

            var pareceristas = new List<PropostaParecerista>
            {
                pareceristaAtual,
                new PropostaParecerista { Situacao = SituacaoParecerista.AguardandoValidacao } // Parecerista pendente
            };

            ConfigurarMocksDeBusca(comando, proposta, pareceristaAtual, pareceristas);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _repositorioPropostaMock.Verify(r => r.AtualizarSituacaoParecerista(
                pareceristaAtual.Id, pareceristaAtual.RegistroFuncional, comando.Situacao, comando.Justificativa), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota.Equals(RotasRabbit.NotificarDFPeloEnvioParecerPeloParecerista)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoPropostaAguardandoAnaliseSemPareceristasPendentes_QuandoHandle_EntaoAtualizaSituacaoPropostaENotificaViaRabbitMq()
        {
            // Arrange
            var comando = GerarComandoValido();
            var proposta = new Proposta { Id = comando.PropostaId, Situacao = SituacaoProposta.AguardandoAnalisePeloParecerista };
            var pareceristaAtual = new PropostaParecerista { Id = 1, RegistroFuncional = comando.RegistroFuncional };

            var pareceristas = new List<PropostaParecerista>
            {
                pareceristaAtual,
                new PropostaParecerista { Situacao = SituacaoParecerista.Enviada } // Sem pendências
            };

            ConfigurarMocksDeBusca(comando, proposta, pareceristaAtual, pareceristas);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(It.Is<EnviarPropostaCommand>(c =>
                c.Situacao == SituacaoProposta.AguardandoAnaliseParecerPelaDF), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<SalvarPropostaMovimentacaoCommand>(c =>
                c.Situacao == SituacaoProposta.AguardandoAnaliseParecerPelaDF), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota.Equals(RotasRabbit.NotificarDFPeloEnvioParecerPeloParecerista)), It.IsAny<CancellationToken>()), Times.Once);
        }

        private EnviarParecerPareceristaCommand GerarComandoValido()
        {
            return new EnviarParecerPareceristaCommand(
                propostaId: _faker.Random.Long(1, 1000),
                registroFuncional: _faker.Random.String2(7, "0123456789"),
                situacao: SituacaoParecerista.Aprovada,
                justificativa: _faker.Lorem.Sentence()
            );
        }

        private void ConfigurarMocksDeBusca(EnviarParecerPareceristaCommand comando, Proposta proposta, PropostaParecerista parecerista, List<PropostaParecerista> pareceristas)
        {
            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == comando.PropostaId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(proposta);

            _mediatorMock.Setup(m => m.Send(It.Is<ObterPropostaPareceristaPorPropostaIdRegistroFuncionalQuery>(q =>
                            q.PropostaId == comando.PropostaId && q.RegistroFuncional == comando.RegistroFuncional), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(parecerista);

            _repositorioPropostaMock.Setup(r => r.ObterPareceristasPorId(proposta.Id))
                                    .ReturnsAsync(pareceristas);

            _mapperMock.Setup(m => m.Map<PropostaPareceristaResumidoDTO>(It.IsAny<PropostaParecerista>()))
                       .Returns(new PropostaPareceristaResumidoDTO());
        }
    }
}