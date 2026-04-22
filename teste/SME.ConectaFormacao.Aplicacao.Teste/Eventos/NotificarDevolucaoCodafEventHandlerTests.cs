using AutoMapper;
using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Eventos.Codaf;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Eventos
{
    public class NotificarDevolucaoCodafEventHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly NotificarDevolucaoCodafEventHandler _handler;
        private readonly Faker _faker;

        public NotificarDevolucaoCodafEventHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<NotificarDevolucaoCodafEventHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoMovimentacaoNaoEncontrada_QuandoHandle_EntaoDeveEncerrarProcessamento()
        {
            // Arrange
            var evento = GerarEventoValido();

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mocker.Verify<IRepositorioUsuario>(x => x.ObterPorLogin(It.IsAny<string>()), Times.Never);
            _mocker.Verify<IRepositorioNotificacao>(x => x.Inserir(It.IsAny<Notificacao>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuariosNaoEncontrados_QuandoHandle_EntaoDeveEncerrarProcessamento()
        {
            // Arrange
            var evento = GerarEventoValido();
            var movimentacao = new CodafMovimentacaoListaPresenca { CriadoLogin = "admin" };

            _mocker.GetMock<IRepositorioCodafMovimentacaoListaPresenca>()
                   .Setup(x => x.ObterUltimaMovimentacaoPorListaPresencaStatusAsync(It.IsAny<long>(), It.IsAny<StatusCodafListaPresenca>()))
                   .ReturnsAsync(movimentacao);

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mocker.Verify<IRepositorioNotificacao>(x => x.Inserir(It.IsAny<Notificacao>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroNaPersistenciaDaNotificacao_Quando_Handle_Entao_NaoDeveEnviarEmailRabbit()
        {
            // Arrange
            var evento = GerarEventoValido();
            ConfigurarMocksDeDadosBasicos(out var _, out var _);

            // Simula transação de notificação
            var transacaoMock = new Mock<IDbTransaction>();
            _mocker.GetMock<ITransacao>()
                   .Setup(x => x.Iniciar())
                   .Returns(transacaoMock.Object);

            // Simula ERRO ao inserir notificação
            _mocker.GetMock<IRepositorioNotificacao>()
                   .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                   .ThrowsAsync(new Exception("Erro de banco"));

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            _mocker.Verify<IMediator>(
                x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DadoFluxoComSucesso_QuandoHandle_EntaoDevePersistirNotificacaoEEnviarRabbit()
        {
            // Arrange
            var evento = GerarEventoValido();
            ConfigurarMocksDeDadosBasicos(out var usuarioAlvo, out var usuarioLogado);

            // Mock Transação
            var transacaoMock = new Mock<IDbTransaction>();
            _mocker.GetMock<ITransacao>()
                   .Setup(x => x.Iniciar())
                   .Returns(transacaoMock.Object);

            // Mock AutoMapper retornando um DTO válido
            _mocker.GetMock<IMapper>()
                   .Setup(x => x.Map<EnviarEmailDto>(It.IsAny<NotificacaoUsuario>()))
                   .Returns(new EnviarEmailDto { EmailDestinatario = usuarioAlvo.Email });

            // Act
            await _handler.Handle(evento, CancellationToken.None);

            // Assert
            // 1. Verifica persistência da notificação
            _mocker.Verify<IRepositorioNotificacao>(x => x.Inserir(It.Is<Notificacao>(n =>
                n.Titulo.Contains($"O CODAF para a formação {evento.CodafListaPresenca.Proposta.NumeroHomologacao} - {evento.CodafListaPresenca.Proposta.NomeFormacao}, turma {evento.CodafListaPresenca.PropostaTurma.Nome} foi devolvida pela DF pelo usuário {usuarioLogado.Nome}") &&
                n.TipoOrigem == NotificacaoTipoOrigem.DevolucaoParaCorrecaoCodaf
            )), Times.Once);

            _mocker.Verify<IRepositorioNotificacaoUsuario>(x => x.InserirUsuarios(
                It.IsAny<IDbTransaction>(),
                It.IsAny<IEnumerable<NotificacaoUsuario>>(),
                It.IsAny<long>()
            ), Times.Once);

            // 2. Verifica Commit da transação leve
            transacaoMock.Verify(x => x.Commit(), Times.Once);

            // 3. Verifica envio para RabbitMQ
            _mocker.Verify<IMediator>(x => x.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.EnviarEmail &&
                ((EnviarEmailDto)c.Filtros).EmailDestinatario == usuarioAlvo.Email
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- Helpers Privados para reduzir repetição no Arrange ---

        private CodafListaPresencaDevolvidaEvento GerarEventoValido()
        {
            var comentario = new CodafComentarioListaPresenca
            {
                Id = _faker.Random.Long(1, 100),
                Comentario = _faker.Lorem.Sentence(),
                NotificacaoCorrelacaoId = Guid.NewGuid()
            };

            var codaf = new CodafListaPresenca(1, 1, new(null, null, null, null, null, null, null), null)
            {
                Id = _faker.Random.Long(1, 100),
                Proposta = new() { NumeroHomologacao = _faker.Random.Long(1000, 9999), NomeFormacao = _faker.Lorem.Sentence() },
                PropostaTurma = new() { Nome = _faker.Lorem.Word() }
            };
            return new CodafListaPresencaDevolvidaEvento(codaf, comentario);
        }

        private void ConfigurarMocksDeDadosBasicos(out Usuario usuarioAlvo, out Usuario usuarioLogado)
        {
            var movimentacao = new CodafMovimentacaoListaPresenca { CriadoLogin = _faker.Internet.UserName() };

            usuarioAlvo = new Usuario
            {
                Login = movimentacao.CriadoLogin,
                Nome = _faker.Name.FullName(),
                Email = _faker.Internet.Email()
            };

            var loginUsuarioAlvo = usuarioAlvo.Login;

            usuarioLogado = new Usuario
            {
                Login = "admin.sme",
                Nome = "Admin SME"
            };

            // Mock Movimentação
            _mocker.GetMock<IRepositorioCodafMovimentacaoListaPresenca>()
                   .Setup(x => x.ObterUltimaMovimentacaoPorListaPresencaStatusAsync(It.IsAny<long>(), It.IsAny<StatusCodafListaPresenca>()))
                   .ReturnsAsync(movimentacao);

            // Mock Usuario Alvo (Banco)
            _mocker.GetMock<IRepositorioUsuario>()
                   .Setup(x => x.ObterPorLogin(loginUsuarioAlvo))
                   .ReturnsAsync(usuarioAlvo);

            // Mock Usuario Logado (Mediator Query)
            _mocker.GetMock<IMediator>()
                   .Setup(x => x.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(usuarioLogado);
        }
    }
}