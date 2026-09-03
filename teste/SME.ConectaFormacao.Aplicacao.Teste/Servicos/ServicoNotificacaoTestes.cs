using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Servicos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Data;

namespace SME.ConectaFormacao.Aplicacao.Teste.Servicos
{
    public class ServicoNotificacaoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ServicoNotificacao _sut;
        private readonly Mock<IDbTransaction> _transacaoMock;
        private readonly Faker _faker;

        public ServicoNotificacaoTestes()
        {
            _mocker = new AutoMocker();
            _transacaoMock = new Mock<IDbTransaction>();
            _faker = new Faker("pt_BR");

            _mocker.GetMock<ITransacao>()
                .Setup(x => x.Iniciar())
                .Returns(_transacaoMock.Object);

            _sut = _mocker.CreateInstance<ServicoNotificacao>();
        }

        [Fact]
        public async Task DadoNotificacaoValida_QuandoPersistirEEnviar_EntaoDevePersistirComSucessoEEnviarEmails()
        {
            // Arrange
            var notificacao = CriarNotificacao();
            var notificacaoId = _faker.Random.Long(1, 1000);

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ReturnsAsync(notificacaoId);

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Setup(x => x.InserirUsuarios(It.IsAny<IDbTransaction>(), It.IsAny<IEnumerable<NotificacaoUsuario>>(), It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<EnviarEmailDto>(It.IsAny<NotificacaoUsuario>()))
                .Returns((NotificacaoUsuario u) => new EnviarEmailDto { EmailDestinatario = u.Email, NomeDestinatario = u.Nome });

            // Act
            var resultado = await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            _mocker.GetMock<IRepositorioNotificacao>()
                .Verify(x => x.Inserir(It.Is<Notificacao>(n => n == notificacao)), Times.Once);

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Verify(x => x.InserirUsuarios(_transacaoMock.Object, notificacao.Usuarios, notificacaoId), Times.Once);

            _transacaoMock.Verify(x => x.Commit(), Times.Once);
            _transacaoMock.Verify(x => x.Rollback(), Times.Never);

            // Verifica envio de emails (removendo duplicatas, espera-se 2 emails únicos)
            _mocker.GetMock<IMediator>()
                .Verify(x => x.Send(
                    It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.EnviarEmail),
                    It.IsAny<CancellationToken>()),
                    Times.Exactly(2));
        }

        [Fact]
        public async Task DadoNotificacaoComUsuariosDuplicados_QuandoPersistirEEnviar_EntaoDeveEnviarApenasEmailsUnicos()
        {
            // Arrange
            var emailDuplicado = _faker.Internet.Email();
            var notificacao = CriarNotificacao(emailDuplicado);

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ReturnsAsync(_faker.Random.Long(1, 1000));

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Setup(x => x.InserirUsuarios(It.IsAny<IDbTransaction>(), It.IsAny<IEnumerable<NotificacaoUsuario>>(), It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<EnviarEmailDto>(It.IsAny<NotificacaoUsuario>()))
                .Returns((NotificacaoUsuario u) => new EnviarEmailDto { EmailDestinatario = u.Email, NomeDestinatario = u.Nome });

            // Act
            var resultado = await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            // Verifica que apenas 1 email foi enviado (removeu duplicatas)
            _mocker.GetMock<IMediator>()
                .Verify(x => x.Send(
                    It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.EnviarEmail),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoErroNaPersistencia_QuandoPersistirEEnviar_EntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var notificacao = CriarNotificacao();
            var excecaoEsperada = new Exception("Erro ao inserir notificação");

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            Func<Task> acao = async () => await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro ao inserir notificação");

            _transacaoMock.Verify(x => x.Rollback(), Times.Once);
            _transacaoMock.Verify(x => x.Commit(), Times.Never);
            _transacaoMock.Verify(x => x.Dispose(), Times.Once);

            // Verifica que nenhum email foi enviado
            _mocker.GetMock<IMediator>()
                .Verify(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoErroAoInserirUsuarios_QuandoPersistirEEnviar_EntaoDeveFazerRollbackELancarExcecao()
        {
            // Arrange
            var notificacao = CriarNotificacao();
            var excecaoEsperada = new Exception("Erro ao inserir usuários");

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ReturnsAsync(_faker.Random.Long(1, 1000));

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Setup(x => x.InserirUsuarios(It.IsAny<IDbTransaction>(), It.IsAny<IEnumerable<NotificacaoUsuario>>(), It.IsAny<long>()))
                .ThrowsAsync(excecaoEsperada);

            // Act
            Func<Task> acao = async () => await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            await acao.Should().ThrowAsync<Exception>().WithMessage("Erro ao inserir usuários");

            _transacaoMock.Verify(x => x.Rollback(), Times.Once);
            _transacaoMock.Verify(x => x.Commit(), Times.Never);

            // Verifica que nenhum email foi enviado
            _mocker.GetMock<IMediator>()
                .Verify(x => x.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoNotificacaoComUsuariosSemEmail_QuandoPersistirEEnviar_EntaoNaoDeveEnviarEmailsParaUsuariosSemEmail()
        {
            // Arrange
            var notificacao = new Notificacao
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Titulo = _faker.Lorem.Sentence(),
                Mensagem = _faker.Lorem.Paragraph(),
                Usuarios = new List<NotificacaoUsuario>
                {
                    new(_faker.Person.FullName, _faker.Internet.Email()), // Com email
                    new(_faker.Person.FullName, string.Empty),            // Sem email
                    new(_faker.Person.FullName, null!)                    // Email nulo
                }
            };

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ReturnsAsync(_faker.Random.Long(1, 1000));

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Setup(x => x.InserirUsuarios(It.IsAny<IDbTransaction>(), It.IsAny<IEnumerable<NotificacaoUsuario>>(), It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<EnviarEmailDto>(It.IsAny<NotificacaoUsuario>()))
                .Returns((NotificacaoUsuario u) => new EnviarEmailDto { EmailDestinatario = u.Email, NomeDestinatario = u.Nome });

            // Act
            var resultado = await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            // Verifica que apenas 1 email foi enviado (apenas o usuário com email preenchido)
            _mocker.GetMock<IMediator>()
                .Verify(x => x.Send(
                    It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.EnviarEmail),
                    It.IsAny<CancellationToken>()),
                    Times.Once);
        }

        [Fact]
        public async Task DadoTransacaoIniciada_QuandoPersistirEEnviar_EntaoDeveDisporTransacaoAoFinalizar()
        {
            // Arrange
            var notificacao = CriarNotificacao();

            _mocker.GetMock<IRepositorioNotificacao>()
                .Setup(x => x.Inserir(It.IsAny<Notificacao>()))
                .ReturnsAsync(_faker.Random.Long(1, 1000));

            _mocker.GetMock<IRepositorioNotificacaoUsuario>()
                .Setup(x => x.InserirUsuarios(It.IsAny<IDbTransaction>(), It.IsAny<IEnumerable<NotificacaoUsuario>>(), It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            _mocker.GetMock<IMapper>()
                .Setup(x => x.Map<EnviarEmailDto>(It.IsAny<NotificacaoUsuario>()))
                .Returns((NotificacaoUsuario u) => new EnviarEmailDto { EmailDestinatario = u.Email, NomeDestinatario = u.Nome });

            // Act
            await _sut.PersistirEEnviarAsync(notificacao, CancellationToken.None);

            // Assert
            _transacaoMock.Verify(x => x.Dispose(), Times.Once);
        }

        private Notificacao CriarNotificacao(string? emailDuplicado = null)
        {
            var email1 = emailDuplicado ?? _faker.Internet.Email();
            var email2 = emailDuplicado ?? _faker.Internet.Email();

            return new Notificacao
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Titulo = _faker.Lorem.Sentence(),
                Mensagem = _faker.Lorem.Paragraph(),
                Usuarios = new List<NotificacaoUsuario>
                {
                    new(_faker.Person.FullName, email1),
                    new(_faker.Person.FullName, email2)
                }
            };
        }
    }
}
