using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacaoTestes
    {
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<TimeProvider> _timeProviderMock;
        private readonly Mock<IServicoLogs> _servicoLogsMock;

        private readonly CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao _sut;
        private readonly Faker _faker;

        public CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacaoTestes()
        {
            var mocker = new AutoMocker();

            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _timeProviderMock = mocker.GetMock<TimeProvider>();
            _servicoLogsMock = mocker.GetMock<IServicoLogs>();

            _sut = mocker.CreateInstance<CasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutarAsync_EntaoDevePublicarMensagemNaFilaERetornarSucesso()
        {
            // Arrange
            var filtro = GerarFiltroValido();
            var usuarioLogado = GerarUsuarioValido();
            var dataAtual = DateTimeOffset.UtcNow;

            ConfigurarContextoBasico(usuarioLogado.Login, usuarioLogado.Nome);

            _repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(It.IsAny<string>()))
                .ReturnsAsync(usuarioLogado);

            _timeProviderMock.Setup(t => t.GetUtcNow()).Returns(dataAtual);
            _timeProviderMock.Setup(t => t.LocalTimeZone).Returns(TimeZoneInfo.Local);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeTrue();
            resultado.TipoFalha.Should().Be(TipoFalha.Nenhuma);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.GerarRelatorioInscritosExcel &&
                c.Usuario == usuarioLogado), It.IsAny<CancellationToken>()), Times.Once);

            _servicoLogsMock.Verify(s => s.Enviar(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<LogContexto>(), It.IsAny<LogNivel>(), 
                It.IsAny<string>())
            , Times.Never);
        }

        [Fact]
        public async Task DadoFalhaNaPublicacao_QuandoExecutarAsync_EntaoDeveRetornarErroInterno()
        {
            // Arrange
            var filtro = GerarFiltroValido();
            var usuarioLogado = GerarUsuarioValido();

            ConfigurarContextoBasico(usuarioLogado.Login, usuarioLogado.Nome);

            _repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(It.IsAny<string>()))
                .ReturnsAsync(usuarioLogado);

            _timeProviderMock.Setup(t => t.GetUtcNow()).Returns(DateTimeOffset.UtcNow);
            _timeProviderMock.Setup(t => t.LocalTimeZone).Returns(TimeZoneInfo.Local);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), default))
                .ReturnsAsync(false);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            resultado.MensagensErro.Should().Contain("Erro ao solicitar relatório");

            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), default), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoLancada_QuandoExecutarAsync_EntaoDeveRetornarErroInterno()
        {
            // Arrange
            var filtro = GerarFiltroValido();
            var usuarioLogado = GerarUsuarioValido();
            var excecaoEsperada = new Exception("Erro de conexão com o RabbitMQ");

            ConfigurarContextoBasico(usuarioLogado.Login, usuarioLogado.Nome);

            _repositorioUsuarioMock
                .Setup(r => r.ObterPorLogin(It.IsAny<string>()))
                .ReturnsAsync(usuarioLogado);

            _timeProviderMock.Setup(t => t.GetUtcNow()).Returns(DateTimeOffset.UtcNow);
            _timeProviderMock.Setup(t => t.LocalTimeZone).Returns(TimeZoneInfo.Local);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), default))
                .ThrowsAsync(excecaoEsperada);

            // Act
            var resultado = await _sut.ExecutarAsync(filtro);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Sucesso.Should().BeFalse();
            resultado.TipoFalha.Should().Be(TipoFalha.ErroInterno);
            resultado.MensagensErro.Should().Contain("Erro ao solicitar relatório");

            _servicoLogsMock.Verify(s => s.Enviar(
                excecaoEsperada,
                It.Is<string>(msg => msg.Contains("Erro ao solicitar geração do relatório")),
                LogContexto.Relatorio,
                LogNivel.Critico,
                It.IsAny<string>()), Times.Once); // Ajustado para corresponder à assinatura da interface IServicoLogs
        }

        #region Métodos Privados Auxiliares

        private void ConfigurarContextoBasico(string login, string nome)
        {
            _contextoAplicacaoMock.Setup(c => c.LoginUsuario).Returns(login);
            _contextoAplicacaoMock.Setup(c => c.NomeUsuario).Returns(nome);
        }

        private FiltroRelatorioInscritosPorFormacaoDto GerarFiltroValido()
        {
            return new FiltroRelatorioInscritosPorFormacaoDto
            {
                PropostaId = _faker.Random.Long(1, 1000),
                NomeFormacao = _faker.Random.String2(10),
                PeriodoDeRealizacaoInicial = DateTime.Now.AddDays(-10),
                PeriodoDeRealizacaoFinal = DateTime.Now.AddDays(10)
            };
        }

        private Usuario GerarUsuarioValido()
        {
            return new Usuario(
                login: _faker.Internet.UserName(),
                nome: _faker.Person.FullName,
                email: _faker.Internet.Email()
            )
            {
                Id = _faker.Random.Long(1, 1000)
            };
        }

        #endregion
    }
}