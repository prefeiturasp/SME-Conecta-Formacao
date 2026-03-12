using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Notificacao.ObterNotificacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Notificacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterNotificacaoQueryHandlerTestes
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioNotificacao> _repositorioNotificacaoMock;
        private readonly Mock<IRepositorioNotificacaoUsuario> _repositorioNotificacaoUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly ObterNotificacaoQueryHandler _sut;
        private readonly DateTimeOffset _dataAtualUtc = new(2026, 3, 10, 10, 0, 0, 0, 1, TimeSpan.Zero);

        public ObterNotificacaoQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioNotificacaoMock = mocker.GetMock<IRepositorioNotificacao>();
            _repositorioNotificacaoUsuarioMock = mocker.GetMock<IRepositorioNotificacaoUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _timeProviderMock = mocker.GetMock<TimeProvider>();

            _timeProviderMock.Setup(t => t.GetUtcNow()).Returns(_dataAtualUtc);

            _sut = mocker.CreateInstance<ObterNotificacaoQueryHandler>();
        }

        [Fact]
        public async Task DadoNotificacaoNaoEncontrada_QuandoHandle_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");

            // Act
            Func<Task> acao = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoNotificacaoExcluida_QuandoHandle_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao { Excluido = true };
            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);

            // Act
            Func<Task> acao = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoNotificacaoUsuarioNaoEncontrada_QuandoHandle_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao { Excluido = false };

            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);

            // Act
            Func<Task> acao = async () => await _sut.Handle(query, CancellationToken.None);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.NOTIFICACAO_NAO_ENCONTRADA_USUARIO);
        }

        [Fact]
        public async Task DadoNotificacaoNaoLida_QuandoHandle_EntaoDeveAtualizarParaLidaEEnviarSignalR()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao { Id = 1, Excluido = false };
            var notificacaoUsuario = new NotificacaoUsuario { Situacao = NotificacaoUsuarioSituacao.NaoLida };
            var notificacaoDto = new NotificacaoDTO { Id = 1 };
            var notificacaoSignalR = new NotificacaoSignalRDTO { Id = 1 };

            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);
            _repositorioNotificacaoUsuarioMock.Setup(r => r.ObterNotificacaoUsuario(query.Id, query.Login))
                                              .ReturnsAsync(notificacaoUsuario);
            _mapperMock.Setup(m => m.Map<NotificacaoDTO>(notificacao)).Returns(notificacaoDto);
            _mapperMock.Setup(m => m.Map<NotificacaoSignalRDTO>(notificacao)).Returns(notificacaoSignalR);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().Be(notificacaoDto);
            notificacaoUsuario.Situacao.Should().Be(NotificacaoUsuarioSituacao.Lida);

            _repositorioNotificacaoUsuarioMock.Verify(r => r.Atualizar(notificacaoUsuario), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarNotificacaoLidaCommand>(c => c.Notificacao == notificacaoSignalR), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoNotificacaoExpirada_QuandoHandle_EntaoDeveSubstituirMensagemPorMensagemAposExpiracao()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao
            {
                Id = 1,
                Excluido = false,
                DataExpiracao = _dataAtualUtc.AddMicroseconds(-1),
                MensagemAposExpiracao = "Conteúdo expirado."
            };
            var notificacaoUsuario = new NotificacaoUsuario { Situacao = NotificacaoUsuarioSituacao.Lida };
            var notificacaoDto = new NotificacaoDTO { Mensagem = "Mensagem original." };

            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);
            _repositorioNotificacaoUsuarioMock.Setup(r => r.ObterNotificacaoUsuario(query.Id, query.Login))
                                              .ReturnsAsync(notificacaoUsuario);
            _mapperMock.Setup(m => m.Map<NotificacaoDTO>(notificacao)).Returns(notificacaoDto);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Mensagem.Should().Be("Conteúdo expirado.");
            _repositorioNotificacaoUsuarioMock.Verify(r => r.Atualizar(It.IsAny<NotificacaoUsuario>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarNotificacaoLidaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoNotificacaoExpiradaSemMensagemCustomizada_QuandoHandle_EntaoDeveSubstituirMensagemPorMensagemDefault()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao
            {
                Id = 1,
                Excluido = false,
                DataExpiracao = _dataAtualUtc.AddMicroseconds(-1),
                MensagemAposExpiracao = null
            };
            var notificacaoUsuario = new NotificacaoUsuario { Situacao = NotificacaoUsuarioSituacao.Lida };
            var notificacaoDto = new NotificacaoDTO { Mensagem = "Mensagem original." };

            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);
            _repositorioNotificacaoUsuarioMock.Setup(r => r.ObterNotificacaoUsuario(query.Id, query.Login))
                                              .ReturnsAsync(notificacaoUsuario);
            _mapperMock.Setup(m => m.Map<NotificacaoDTO>(notificacao)).Returns(notificacaoDto);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Mensagem.Should().Be("Mensagem expirada");
        }

        [Fact]
        public async Task DadoNotificacaoValidaEJaLidaEAtiva_QuandoHandle_EntaoDeveRetornarDtoSemAtualizar()
        {
            // Arrange
            var query = new ObterNotificacaoQuery(1, "123456");
            var notificacao = new Notificacao
            {
                Id = 1,
                Excluido = false,
                DataExpiracao = _dataAtualUtc
            };
            var notificacaoUsuario = new NotificacaoUsuario { Situacao = NotificacaoUsuarioSituacao.Lida };
            var notificacaoDto = new NotificacaoDTO { Mensagem = "Mensagem original." };

            _repositorioNotificacaoMock.Setup(r => r.ObterPorId(query.Id)).ReturnsAsync(notificacao);
            _repositorioNotificacaoUsuarioMock.Setup(r => r.ObterNotificacaoUsuario(query.Id, query.Login))
                                              .ReturnsAsync(notificacaoUsuario);
            _mapperMock.Setup(m => m.Map<NotificacaoDTO>(notificacao)).Returns(notificacaoDto);

            // Act
            var resultado = await _sut.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().Be(notificacaoDto);
            resultado.Mensagem.Should().Be("Mensagem original.");
            _repositorioNotificacaoUsuarioMock.Verify(r => r.Atualizar(It.IsAny<NotificacaoUsuario>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarNotificacaoLidaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
