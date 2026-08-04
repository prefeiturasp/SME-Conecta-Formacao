using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Logs
{
    public class SalvarLogCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarLogCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarLogCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<SalvarLogCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoComandoValidoComUsuarioLogadoEExcecaoNula_QuandoHandle_EntaoDeveInserirLogEEnviarServicoSemExcecaoERetornarVerdadeiro()
        {
            // Arrange
            var comando = GerarComandoFake(comExcecao: false);
            var usuario = GerarUsuarioFake();
            var logMapeado = new Log { Entidade = comando.Entidade, NivelLog = comando.NivelLog };

            ConfigurarDependenciasSucesso(comando, usuario, logMapeado);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();

            logMapeado.CriadoPor.Should().Be(usuario.Id.ToString());
            logMapeado.CriadoLogin.Should().Be(usuario.Login);
            logMapeado.Mensagem.Should().Contain(comando.IdentificadorRastreamento.ToString());
            logMapeado.Mensagem.Should().Contain(comando.Mensagem);

            _mocker.GetMock<IRepositorioLog>()
                .Verify(r => r.InserirAsync(logMapeado), Times.Once);

            _mocker.GetMock<IServicoLogs>()
                .Verify(s => s.Enviar(
                    logMapeado.Mensagem,
                    It.IsAny<LogContexto>(),
                    logMapeado.NivelLog,
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoComandoValidoSemUsuarioLogado_QuandoHandle_EntaoDeveAtribuirUsuarioSistemaEInserirLogERetornarVerdadeiro()
        {
            // Arrange
            var comando = GerarComandoFake(comExcecao: false);
            var logMapeado = new Log { Entidade = comando.Entidade, NivelLog = comando.NivelLog };

            // Retorna null para forçar o fallback para o usuário "Sistema"
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<Log>(comando))
                .Returns(logMapeado);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            logMapeado.CriadoPor.Should().Be("1");
            logMapeado.CriadoLogin.Should().Be("Sistema");
        }

        [Fact]
        public async Task DadoComandoValidoComExcecaoEComplemento_QuandoHandle_EntaoDeveFormatarComplementoComExcecaoEEnviarServicoComExcecao()
        {
            // Arrange
            var comando = GerarComandoFake(comExcecao: true);
            var usuario = GerarUsuarioFake();
            var logMapeado = new Log { Entidade = comando.Entidade, NivelLog = comando.NivelLog };

            ConfigurarDependenciasSucesso(comando, usuario, logMapeado);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeTrue();
            logMapeado.Complemento.Should().Contain(comando.Complemento);
            logMapeado.Complemento.Should().Contain(comando.Excecao!.Message);

            _mocker.GetMock<IServicoLogs>()
                .Verify(s => s.Enviar(
                    comando.Excecao,
                    logMapeado.Mensagem,
                    It.IsAny<LogContexto>(),
                    logMapeado.NivelLog,
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task DadoErroInesperadoNoProcesso_QuandoHandle_EntaoDeveEnviarLogDeErroCriticoERetornarFalso()
        {
            // Arrange
            var comando = GerarComandoFake(comExcecao: false);
            var excecaoInesperada = new Exception("Erro de banco de dados");

            // Força um erro no Mediator para acionar o bloco catch externo
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(excecaoInesperada);

            // Act
            var resultado = await _handler.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().BeFalse();

            _mocker.GetMock<IServicoLogs>()
                .Verify(s => s.Enviar(
                    excecaoInesperada,
                    It.Is<string>(msg => msg.Contains(comando.Mensagem)),
                    It.IsAny<LogContexto>(),
                    LogNivel.Critico,
                    It.IsAny<string>()),
                Times.Once);
        }

        #region Metodos Privados Auxiliares

        private SalvarLogCommand GerarComandoFake(bool comExcecao)
        {
            var excecao = comExcecao ? new Exception(_faker.Lorem.Sentence(), new Exception("Inner exception simulada")) : null;

            return new SalvarLogCommand(
                entidade: _faker.Database.Collation(),
                nivelLog: _faker.PickRandom<LogNivel>(),
                mensagem: _faker.Lorem.Sentence(),
                complemento: _faker.Lorem.Paragraph(),
                identificadorRastreamento: Guid.NewGuid(),
                excecao: excecao
            );
        }

        private Usuario GerarUsuarioFake()
        {
            return new Usuario(
                login: _faker.Internet.UserName(),
                nome: _faker.Name.FullName(),
                email: _faker.Internet.Email()
            )
            {
                Id = _faker.Random.Long(1, 1000)
            };
        }

        private void ConfigurarDependenciasSucesso(SalvarLogCommand comando, Usuario usuario, Log logMapeado)
        {
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<Log>(comando))
                .Returns(logMapeado);

            _mocker.GetMock<IRepositorioLog>()
                .Setup(r => r.InserirAsync(It.IsAny<Log>()))
                .ReturnsAsync(1);
        }

        #endregion
    }
}
