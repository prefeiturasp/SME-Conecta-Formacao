using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.Utilitarios
{
    public class UtilitariosCodafTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IServicoLogs> _servicoLogsMock;
        private readonly UtilitariosCodaf _sut;
        private readonly Faker _faker;

        public UtilitariosCodafTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _servicoLogsMock = mocker.GetMock<IServicoLogs>();
            _sut = mocker.CreateInstance<UtilitariosCodaf>();
            _faker = new Faker("pt_BR");
        }

        [Fact(DisplayName = "Dado lista de emails sem duplicatas - Quando enviar emails - Então deve enviar para todos os destinatários")]
        public async Task DadoListaEmailsSemDuplicatas_QuandoEnviarEmails_EntaoDeveEnviarParaTodosDestinatarios()
        {
            // Arrange
            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = _faker.Internet.Email(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = _faker.Internet.Email(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 2", Texto = "Mensagem 2" },
                new() { EmailDestinatario = _faker.Internet.Email(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 3", Texto = "Mensagem 3" }
            };

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact(DisplayName = "Dado lista com emails duplicados - Quando enviar emails - Então deve enviar apenas uma vez por destinatário")]
        public async Task DadoListaComEmailsDuplicados_QuandoEnviarEmails_EntaoDeveEnviarApenasUmaVezPorDestinatario()
        {
            // Arrange
            var emailDuplicado = _faker.Internet.Email();
            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = emailDuplicado, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = emailDuplicado, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = _faker.Internet.Email(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 2", Texto = "Mensagem 2" }
            };

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "Dado lista com emails duplicados em case diferente - Quando enviar emails - Então deve considerar como duplicata")]
        public async Task DadoListaComEmailsDuplicadosEmCaseDiferente_QuandoEnviarEmails_EntaoDeveConsiderarComoDuplicata()
        {
            // Arrange
            var emailBase = _faker.Internet.Email();
            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = emailBase.ToLower(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = emailBase.ToUpper(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = emailBase, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" }
            };

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Dado lista com emails com espaços - Quando enviar emails - Então deve considerar como duplicata após trim")]
        public async Task DadoListaComEmailsComEspacos_QuandoEnviarEmails_EntaoDeveConsiderarComoDuplicataAposTrim()
        {
            // Arrange
            var emailBase = _faker.Internet.Email();
            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = $" {emailBase} ", NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = emailBase, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = $"{emailBase}  ", NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" }
            };

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Dado lista com emails vazios ou nulos - Quando enviar emails - Então deve ignorar e não enviar")]
        public async Task DadoListaComEmailsVaziosOuNulos_QuandoEnviarEmails_EntaoDeveIgnorarENaoEnviar()
        {
            // Arrange
            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = string.Empty, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = null!, NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 2", Texto = "Mensagem 2" },
                new() { EmailDestinatario = "   ", NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 3", Texto = "Mensagem 3" },
                new() { EmailDestinatario = _faker.Internet.Email(), NomeDestinatario = _faker.Person.FullName, Titulo = "Teste 4", Texto = "Mensagem 4" }
            };

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Dado lista com múltiplas duplicatas - Quando enviar emails - Então deve enviar apenas primeira ocorrência de cada email")]
        public async Task DadoListaComMultiplasDuplicatas_QuandoEnviarEmails_EntaoDeveEnviarApenasPrimeiraOcorrenciaDeCadaEmail()
        {
            // Arrange
            var email1 = _faker.Internet.Email();
            var email2 = _faker.Internet.Email();
            var email3 = _faker.Internet.Email();

            var notificacoes = new List<EnviarEmailDto>
            {
                new() { EmailDestinatario = email1, NomeDestinatario = "Nome1", Titulo = "Teste 1", Texto = "Mensagem 1" },
                new() { EmailDestinatario = email2, NomeDestinatario = "Nome2", Titulo = "Teste 2", Texto = "Mensagem 2" },
                new() { EmailDestinatario = email1, NomeDestinatario = "Nome1Duplicado", Titulo = "Teste 1 Dup", Texto = "Mensagem 1 Dup" },
                new() { EmailDestinatario = email3, NomeDestinatario = "Nome3", Titulo = "Teste 3", Texto = "Mensagem 3" },
                new() { EmailDestinatario = email2, NomeDestinatario = "Nome2Duplicado", Titulo = "Teste 2 Dup", Texto = "Mensagem 2 Dup" },
                new() { EmailDestinatario = email1, NomeDestinatario = "Nome1Duplicado2", Titulo = "Teste 1 Dup2", Texto = "Mensagem 1 Dup2" }
            };

            EnviarEmailDto? emailCapturado = null;

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((command, ct) =>
                {
                    if (command is PublicarNaFilaRabbitCommand rabbitCommand && rabbitCommand.Filtros is EnviarEmailDto dto)
                    {
                        if (dto.EmailDestinatario == email1 && emailCapturado == null)
                            emailCapturado = dto;
                    }
                })
                .ReturnsAsync(true);

            // Act
            await _sut.EnviarEmailsAsync(notificacoes);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

            // Verifica que a primeira ocorrência foi mantida
            Assert.NotNull(emailCapturado);
            Assert.Equal("Nome1", emailCapturado.NomeDestinatario);
        }
    }
}
